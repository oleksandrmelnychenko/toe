using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TicTacToeServer
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

        public async Task StartServerAsync()
        {
            try
            {
                listener.Start();
                IsActive = true;
                Console.WriteLine("Server started.");
                await AcceptClientsAsync();
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
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    clients.Add(client);
                    Console.WriteLine("Client connected.");
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

                await Task.WhenAny(clientTasks);
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
                var buffer = new byte[1024];

                while (client.Connected)
                {
                    int received = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (received == 0)
                    {
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, received);
                    Console.WriteLine($"Received message: {message}");
                    message = ProcessMessage(message);

                    if (!string.IsNullOrEmpty(message))
                    {
                        await SendDataToClientsAsync(message);
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
            finally
            {
                clients.Remove(client);
                client.Close();
            }
        }

        public async Task SendDataToClientsAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);

            foreach (TcpClient client in clients)
            {
                if (client.Connected)
                {
                    try
                    {
                        await client.GetStream().WriteAsync(bytes, 0, bytes.Length);
                        Console.WriteLine("Data has been sent to the client");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Server cannot send data: {ex}");
                        client.Close();
                    }
                }
                else
                {
                    Console.WriteLine("Client is not connected to the server.");
                }
            }
        }

        private string ProcessMessage(string message)
        {
            return $"Processed: {message}";
        }
    }
}
