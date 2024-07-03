using System.Net;
using System.Net.Sockets;
using System.Text;
using Tic_tac_toe_Server.Logging;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Net;

namespace TicTacToeServer
{
    public class Server
    {
        public TcpListener listener;
        public bool IsActive { get; private set; } = false;

        private ILogger logger;

        private const int clientsNumber = 2;
        private List<TcpClient> clients = new List<TcpClient>(clientsNumber);

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
                logger.LogMessage("\nServer started.\n");
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

        //Accept clients while clients.Count < 2
        public async Task AcceptClientsAsync()
        {
            try
            {
                while (clients.Count < clientsNumber)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    clients.Add(client);
                    logger.LogMessage("\nClient connected.\n");
                }
                logger.LogMessage("\nAll clients connected.\n");
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

                foreach (TcpClient client in clients)
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
        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                var buffer = new byte[1024];

                while (client.Connected)
                {
                    int received = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (received == 0)
                    {
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, received);
                    //Console.WriteLine($"Received message: {message}");
                    message = ProcessMessage(message);

                    if (!string.IsNullOrEmpty(message))
                    {
                        await SendDataToClientsAsync(message);
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
                client.Close();
            }
        }

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

        private string ProcessMessage(string message)
        {
            PlayerMove move = JsonDataSerializer.DeserializeMove(message);
            return $"{message}";
        }
    }
}
