using System.Net;
using System.Net.Sockets;
using System.Text;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net.Messages;
using TicTacToeGame.Client.Net;

namespace Tic_tac_toe_Server.Net
{
    public class Server : IDisposable
    {
        private readonly object _clientLock = new();

        private CancellationTokenSource cancellationTokenSource = new();

        private readonly ILogger _logger;

        private bool _disposed = false;

        private Socket _networkEndPoint;

        private List<RemotePeer> _clients = new();

        public event Action<string> MessageReceived = delegate{ };

        public Server(IPEndPoint iPEndPoint, ILogger logger)
        {
            _logger = logger;
            _networkEndPoint = Initialize(iPEndPoint);
        }

        public void StartServer()
        {
            try
            {
                _networkEndPoint.Listen();
                _logger.LogSuccess("Server start successful.");
                Task.Run(() => AcceptClientsAsync(cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                _networkEndPoint.Shutdown(SocketShutdown.Both);
                _networkEndPoint.Close();
                _networkEndPoint.Dispose();
            }
        }

        public void StopServer()
        {
            try
            {
                cancellationTokenSource.Cancel();
                _networkEndPoint.Shutdown(SocketShutdown.Both);
                _networkEndPoint.Close();
                _logger.LogSuccess("Server stop successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                _networkEndPoint.Dispose();
            }
        }

        //Це не працює
        public async Task SendDataToClients(List<Guid> clientsIds, string message)
        {
            List<RemotePeer> clients = _clients.Where(c => clientsIds.Contains(c.Id)).ToList();

            foreach (RemotePeer client in clients)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message + "\n");

                if (client.Socket.Connected)
                {
                    try
                    {
                        NetworkStream networkStream = new NetworkStream(client.Socket);

                        await networkStream.WriteAsync(bytes, 0, bytes.Length);

                        _logger.LogMessage($"Message send to clients: {client.Id}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }
                else
                {
                    _logger.LogWarning($"Client {client.Id} is not connected.");
                }
            }
        }

        public async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Socket socket = await _networkEndPoint.AcceptAsync().ConfigureAwait(false);

                    RemotePeer client = new(socket);

                    lock (_clientLock)
                    {
                        _clients.Add(client);
                    }

                    client.DataReceived += Client_DataReceived;
                    _logger.LogMessage($"Client {client.Id} connected.");

                    await SendInitializeClientDataAsync(client).ConfigureAwait(false);
                    _ = Task.Run(() => client.StartReceiveAsync(), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogMessage("Client acceptance was canceled.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception while accepting client: {ex.Message}");
                }
            }
        }

        private void Client_DataReceived(object sender, string data)
        {
            try
            {
                MessageReceived?.Invoke(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in MessageReceived handler: {ex.Message}");
            }
        }

        private async Task SendInitializeClientDataAsync(RemotePeer client)
        {
            PlayerInitializationConfig config = new PlayerInitializationConfig(client.Id);
            JsonValidationResult jsonValidationResult = Serializer.Serialize(config);

            if (jsonValidationResult.IsValid)
            {
                await SendDataToClient(client, jsonValidationResult.JsonMessage);
                _logger.LogMessage($"Initialize data send to client {client.Id}");
            }
            else
            {
                _logger.LogError($"Serialization problem for client {client.Id}: {jsonValidationResult.JsonMessage}");

                await SendDataToClient(client, "An error occurred during initialization. Please try again.");

                _clients.Remove(client);
                client.Socket.Dispose();
                _logger.LogMessage($"Client {client.Id} disconnected due to serialization error.");
            }
        }

        private Socket Initialize(IPEndPoint endPointAddress)
        {
            Socket networkEndPoint = new Socket(SocketType.Stream, ProtocolType.Tcp);

            try
            {
                networkEndPoint.Bind(endPointAddress);
                return networkEndPoint;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private async Task SendDataToClient(RemotePeer client, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\n");

            if (client.Socket.Connected)
            {
                try
                {
                    NetworkStream networkStream = new NetworkStream(client.Socket);

                    await networkStream.WriteAsync(bytes, 0, bytes.Length);

                    _logger.LogMessage($"Message send to client: {client.Id}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            else
            {
                _logger.LogWarning($"Client {client.Id} is not connected.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var client in _clients)
                    {
                        client.DataReceived -= Client_DataReceived;
                        client.Dispose();
                    }

                    if (_networkEndPoint.Connected)
                    {
                        _networkEndPoint.Shutdown(SocketShutdown.Both);
                        _networkEndPoint.Dispose();
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
