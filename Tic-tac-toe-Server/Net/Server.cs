using System.Net;
using System.Net.Sockets;
using System.Text;
using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Logging;

namespace Tic_tac_toe_Server.Net
{
    public class Server : IDisposable
    {
        private ILogger _logger;

        private bool _disposed;

        private const int _clientsNumber = 2;

        private List<Client> _clients = new List<Client>(_clientsNumber);

        private TcpListener _listener;

        private bool _isAllClientConnected = false;

        private PlayerManager _playerManager;

        public bool IsActive { get; private set; } = false;

        public event EventHandler<string> MessageReceived;

        public event EventHandler AllClientsReconnected;

        public Server(IPAddress address, int port, ILogger logger)
        {
            this._logger = logger;
            _listener = new TcpListener(address, port);
        }

        public async Task StartServerAsync()
        {
            try
            {
                _listener.Start();
                IsActive = true;
                _logger.LogMessage("Server started.\n");
                await AcceptClientsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"\nAn error occurred: {ex.Message}\n");
            }
        }

        public void StopServer()
        {
            try
            {
                _listener.Stop();
                IsActive = false;
                _logger.LogMessage("\nServer stopped.\n");
            }
            catch (Exception ex)
            {
                _logger.LogError($"\nAn error occurred while stopping the server: {ex.Message}\n");
            }
        }

        public void SetPlayerManager(PlayerManager playerManager)
        {
            this._playerManager = playerManager;
        }

        /// <summary>
        /// Waiting for clients to connect, after one of the clients connects, it sends the player's data
        /// </summary>
        public async Task AcceptClientsAsync()
        {
            try
            {
                while (_clients.Count < _clientsNumber)
                {
                    TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                    Client client = new Client(tcpClient);

                    _clients.Add(client);

                    _playerManager.ConnectClientToPlayer(ref client);

                    var serializedPlayerId = ServerJsonDataSerializer.SerializePlayerId(client.Id);

                    _logger.LogMessage($"Sending initial data to client {_clients.Count}: {serializedPlayerId}");

                    await SendDataToClientAsync(client, serializedPlayerId);

                    _logger.LogMessage($"Client {_clients.Count} connected and initial data sent.");
                }
                _logger.LogSuccess("\nAll clients connected.\n");
                _isAllClientConnected = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"\nAn error occurred while accepting clients: {ex.Message}\n");
            }
        }


        /// <summary>
        /// Listens messages from customers
        /// </summary>
        public async Task ListenClientsAsync()
        {
            if (!IsActive)
            {
                _logger.LogWarning("\nServer is not active.\n");
                return;
            }

            try
            {
                List<Task> clientTasks = new List<Task>();

                foreach (Client client in _clients)
                {
                    clientTasks.Add(HandleClientAsync(client));
                }

                await Task.WhenAny(clientTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"\nGeneral problem with reading data from client: {ex.Message}\n");
            }
        }

        /// <summary>
        /// Handles messages from customers
        /// </summary>
        /// <param name="client"></param>
        private async Task HandleClientAsync(Client client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                var buffer = new byte[1024];

                while (client.Connected && _isAllClientConnected)
                {
                    int received = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (received == 0)
                    {
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, received);

                    if (!string.IsNullOrEmpty(message))
                    {
                        MessageReceived?.Invoke(this, message);
                    }
                }
            }
            catch (IOException ex)
            {
                _logger.LogError($"\nIO problem with reading data from client: {ex.Message}\n");
            }
            catch (Exception ex)
            {
                _logger.LogError($"\nGeneral problem with reading data from client: {ex.Message}\n");
            }
            finally
            {
                _clients.Remove(client);

                _logger.LogWarning($"\nClient {client.Id} disconected!\n");

                _playerManager.DisconnectClientToPlayer(client.Id);
           
                client.Dispose();

                _isAllClientConnected = false;

                await AcceptClientsAsync();

                //Супер костиль але чомусь в мене івент викликається одразу, під час виклику івент передаються рестартові дані,
                //а в методі AcceptClientsAsync передаються дані ід клієнта, і вони змішуються в одному повідомленні через що і помилка.
                Thread.Sleep(20);

                AllClientsReconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     SendDataToClientsAsync sends data to all clients
        /// </summary>
        public async Task SendDataToClientsAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\n");

            foreach (TcpClient client in _clients)
            {
                if (client.Connected)
                {
                    try
                    {
                        await client.GetStream().WriteAsync(bytes, 0, bytes.Length);
                        _logger.LogMessage("\nData has been sent to the client\n");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"\nServer cannot send data: {ex}\n");
                        client.Close();
                    }
                }
                else
                {
                    _logger.LogWarning("\nClient is not connected to the server.\n");
                }
            }
        }

        /// <summary>
        ///     SendDataToClientsAsync sends data to one client
        /// </summary>
        public async Task SendDataToClientAsync(TcpClient client, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\n");

            if (client.Connected)
            {
                try
                {
                    await client.GetStream().WriteAsync(bytes, 0, bytes.Length);
                    _logger.LogMessage("\nData has been sent to the client\n");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"\nServer cannot send data: {ex}\n");
                    client.Close();
                }
            }
            else
            {
                _logger.LogWarning("\nClient is not connected to the server.\n");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
                if(disposing)
                {
                    foreach(var client in _clients)
                    {
                        client.Close();
                        client.Dispose();
                    }
                    _clients.Clear();

                    this.StopServer();

                    _listener.Dispose();
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
