using System.Net;
using System.Net.Sockets;
using System.Text;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Messages;

namespace Tic_tac_toe_Server.Net
{
    public class Server : IDisposable
    {
        private readonly object _clientLock = new();

        private readonly ILogger _logger;

        private bool _disposed = false;

        private Socket _server;

        private IPEndPoint _ipEndPoint;

        private List<Client> _clients = new List<Client>();

        public event Action<Guid> ClientConnected = delegate { };

        public event Action<string> MessageReceived = delegate { };

        public Server(IPEndPoint iPEndPoint, ILogger logger)
        {
            _logger = logger;
            Initialize(iPEndPoint, ref _server!);
        }

        public void StartServer()
        {
            try
            {
                _server.Listen();
                _logger.LogSuccess("Server start successful.");
                Task.Run(() => AcceptClientsAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                _server.Shutdown(SocketShutdown.Both);
                _server.Close();
                _server.Dispose();
            }
        }

        public void StopServer()
        {
            try
            {
                _server.Shutdown(SocketShutdown.Both);
                _server.Close();
                _logger.LogSuccess("Server stop successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                _server.Dispose();
            }
        }

        public async Task SendDataToClients(List<Guid> clientsIds, string message)
        {
            List<Client> clients = _clients.Where(c => clientsIds.Contains(c.Id)).ToList();

            foreach (Client client in clients)
            {
                var bytes = Encoding.UTF8.GetBytes(message + "\n");

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

        protected virtual void OnClientConnected(Guid clientId)
        {
            ClientConnected?.Invoke(clientId);
        }

        private async Task AcceptClientsAsync()
        {
            while (true)
            {
                try
                {
                    Socket socket = await _server.AcceptAsync();

                    Client client = new(socket);

                    lock (_clientLock)
                    {
                        _clients.Add(client);
                    }

                    client.DataReceived += Client_DataReceived;
                    _logger.LogMessage($"Client {client.Id} connected.");
                    await SendInitializeClientDataAsync(client);
                    _ = Task.Run(() => client.StartReceiveAsync());
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception while accepting client: {ex.Message}");
                }
            }
        }

        private void Client_DataReceived(object sender, string data)
        {
            Client client = (Client)sender;
            try
            {
                MessageReceived?.Invoke(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in MessageReceived handler: {ex.Message}");
            }
        }

        private async Task SendInitializeClientDataAsync(Client client)
        {
            JsonValidationResult jsonValidationResult = Serializer.SerializeClientData(client.Id);

            if (jsonValidationResult.IsValid)
            {
                await SendDataToClient(client, jsonValidationResult.JsonMessage);
                _logger.LogMessage($"Initialize data send to client {client.Id}");
                OnClientConnected(client.Id);
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

        private void Initialize(IPEndPoint iPEndPoint, ref Socket server)
        {
            server = new Socket(SocketType.Stream, ProtocolType.Tcp);

            while (!_server.IsBound)
            {
                (bool isValidEndPoint, string endPointValidationMessage) = EndPointValidation.IsValidEndPoint(iPEndPoint);

                if (isValidEndPoint)
                {
                    try
                    {
                        _server.Bind(iPEndPoint);
                        _ipEndPoint = iPEndPoint;
                        _logger.LogSuccess("Server binding successful.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        UpdateEndPoint(iPEndPoint);
                        continue;
                    }
                }
                else
                {
                    _logger.LogError(endPointValidationMessage);
                    UpdateEndPoint(iPEndPoint);
                    continue;
                }
            }
        }

        private async Task SendDataToClient(Client client, string message)
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

        private void UpdateEndPoint(IPEndPoint iPEndPoint)
        {
            _logger.LogMessage("Please enter new IP address:");
            IPAddress ipAddress;
            if (IPAddress.TryParse(Console.ReadLine(), out ipAddress))
            {
                iPEndPoint.Address = ipAddress;

                _logger.LogMessage("Please enter new port:");
                int port;

                if (int.TryParse(Console.ReadLine(), out port))
                {
                    iPEndPoint.Port = port;
                }
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

                    if (_server.Connected)
                    {
                        _server.Shutdown(SocketShutdown.Both);
                        _server.Dispose();
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
