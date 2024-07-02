using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tic_tac_toe_Server
{
    public class Server
    {
        public TcpListener listener;
        public bool IsActive { get; private set; } = false;

        private const int clientsNumber = 2;
        private List<TcpClient> clients = new List<TcpClient>(clientsNumber);

        public Server(IPAddress address, int port)
        {
            listener = new TcpListener(address, port);
        }

        public void StartServer()
        {
            try
            {
                listener.Start();
                IsActive = true;
                Console.WriteLine("Server started.");
                AcceptClientsAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void StopServer()
        {
            try
            {
                listener.Stop();
                IsActive = false;
                Console.WriteLine("Server stopped.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while stopping the server: {ex.Message}");
            }
        }

        public async Task AcceptClientsAsync()
        {
            try
            {
                while (clients.Count < clientsNumber)
                {
                    TcpClient client = new TcpClient();
                    client = await listener.AcceptTcpClientAsync();
                    clients.Add(client);
                }
                Console.WriteLine("All clients connected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while accepting clients: {ex.Message}");
            }
        }

        public async Task ListenClientsAsync()
        {
            if (!IsActive)
            {
                Console.WriteLine("Server is not active.");
                return;
            }

            try
            {
                List<Task> clientTasks = new List<Task>();

                foreach (TcpClient client in clients)
                {
                    clientTasks.Add(HandleClientAsync(client));
                }

                await Task.WhenAll(clientTasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General problem with reading data from client: {ex.Message}");
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                var buffer = new byte[1_024];

                while (true)
                {
                    int received = await stream.ReadAsync(buffer);
                    if (received == 0)
                    {
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, received);

                    if (!string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine($"{message}");
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO problem with reading data from client: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General problem with reading data from client: {ex.Message}");
            }
        }


    }
}
