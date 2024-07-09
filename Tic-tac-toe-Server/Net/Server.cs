using System.Net;
using System.Net.Sockets;
using System.Text;
using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Logging;

namespace Tic_tac_toe_Server.Net
{
    public class Server : IDisposable
    {
        private ILogger logger;

        private bool _disposed;

        private const int clientsNumber = 2;

        private List<Client> clients = new List<Client>(clientsNumber);

        private TcpListener listener;

        private bool IsAllClientConnected = false;

        private PlayerManager playerManager;

        public bool IsActive { get; private set; } = false;

        public event EventHandler<string> MessageReceived;

        public event EventHandler AllClientsReconnected;

        public Server(IPAddress address, int port, ILogger logger)
        {
            this.logger = logger;
            listener = new TcpListener(address, port);
        }

        public async Task StartServerAsync()
        {
            try
            {
                listener.Start();
                IsActive = true;
                logger.LogMessage("Server started.\n");
                await AcceptClientsAsync();
            }
            catch (Exception ex)
            {
                logger.LogError($"\nAn error occurred: {ex.Message}\n");
            }
        }

        public void StopServer()
        {
            try
            {
                listener.Stop();
                IsActive = false;
                logger.LogMessage("\nServer stopped.\n");
            }
            catch (Exception ex)
            {
                logger.LogError($"\nAn error occurred while stopping the server: {ex.Message}\n");
            }
        }

        public void SetPlayerManager(PlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        public async Task AcceptClientsAsync()
        {
            try
            {
                while (clients.Count < clientsNumber)
                {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    Client client = new Client(tcpClient);

                    clients.Add(client);

                    playerManager.ConnectClientToPlayer(ref client);

                    var serializedPlayerId = ServerJsonDataSerializer.SerializePlayerId(client.Id);

                    logger.LogMessage($"Sending initial data to client {clients.Count}: {serializedPlayerId}");

                    await SendDataToClientAsync(client, serializedPlayerId);

                    logger.LogMessage($"Client {clients.Count} connected and initial data sent.");
                }
                logger.LogSuccess("\nAll clients connected.\n");
                IsAllClientConnected = true;
            }
            catch (Exception ex)
            {
                logger.LogError($"\nAn error occurred while accepting clients: {ex.Message}\n");
            }
        }


        //Listen messages from clients
        public async Task ListenClientsAsync()
        {
            if (!IsActive)
            {
                logger.LogWarning("\nServer is not active.\n");
                return;
            }

            try
            {
                List<Task> clientTasks = new List<Task>();

                foreach (Client client in clients)
                {
                    clientTasks.Add(HandleClientAsync(client));
                }

                await Task.WhenAny(clientTasks);
            }
            catch (Exception ex)
            {
                logger.LogError($"\nGeneral problem with reading data from client: {ex.Message}\n");
            }
        }

        //When get message from one of the clients, processes it and sends it to all clients
        private async Task HandleClientAsync(Client client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                var buffer = new byte[1024];

                while (client.Connected && IsAllClientConnected)
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
                logger.LogError($"\nIO problem with reading data from client: {ex.Message}\n");
            }
            catch (Exception ex)
            {
                logger.LogError($"\nGeneral problem with reading data from client: {ex.Message}\n");
            }
            finally
            {
                clients.Remove(client);

                logger.LogWarning($"\nClient {client.Id} disconected!\n");

                playerManager.DisconnectClientToPlayer(client.Id);
           
                client.Dispose();

                IsAllClientConnected = false;

                await AcceptClientsAsync();

                //Супер костиль але чомусь в мене івент викликається одразу, через що дані передаються не коректно
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

            foreach (TcpClient client in clients)
            {
                if (client.Connected)
                {
                    try
                    {
                        await client.GetStream().WriteAsync(bytes, 0, bytes.Length);
                        logger.LogMessage("\nData has been sent to the client\n");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"\nServer cannot send data: {ex}\n");
                        client.Close();
                    }
                }
                else
                {
                    logger.LogWarning("\nClient is not connected to the server.\n");
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
                    logger.LogMessage("\nData has been sent to the client\n");
                }
                catch (Exception ex)
                {
                    logger.LogError($"\nServer cannot send data: {ex}\n");
                    client.Close();
                }
            }
            else
            {
                logger.LogWarning("\nClient is not connected to the server.\n");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
                if(disposing)
                {
                    foreach(var client in clients)
                    {
                        client.Close();
                        client.Dispose();
                    }
                    clients.Clear();

                    this.StopServer();

                    listener.Dispose();
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
