using System.Net;
using System.Net.Sockets;

namespace Tic_tac_toe_Server
{
    public class Server
    {
        public TcpListener listener;
        private bool IsActive = false;

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


        //public string ListenClients()
        //{
        //    if(IsActive)
        //    {
        //        try
        //        {
        //            TcpClient handler = listener.AcceptTcpClient();
        //            NetworkStream stream = handler.GetStream();

        //            var buffer = new byte[1_024];
        //            int received = stream.Read(buffer);
        //            var message = Encoding.UTF8.GetString(buffer, 0, received);

        //            Console.WriteLine($"{message}");
        //            return message;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Problem with reading data from client: {ex.Message}");
        //            return " ";
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Server is not active.");
        //        return " ";
        //    }
        //}

    }
}
