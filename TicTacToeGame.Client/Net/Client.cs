using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Models;

namespace TicTacToeGame.Client.Net
{
    public class Client
    {
        private TcpClient tcpClient;
        public Guid ClientId { get; init; }

        public User User { get; private set; }

        private NetworkStream stream;

        private IPEndPoint remoteEndPoint;

        public event EventHandler<string> MessageReceived;

        public Client(IPAddress address, int port)
        {
            tcpClient = new TcpClient();
            ClientId = Guid.NewGuid();
            remoteEndPoint = new IPEndPoint(address, port);
        }

        public async Task ConnectAsync()
        {
            try
            {
                await tcpClient.ConnectAsync(remoteEndPoint);
                stream = tcpClient.GetStream();
                Debug.WriteLine($"Client {ClientId} connected to server");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Client {ClientId} cannot connect to server: {ex}");
            }
        }

        public async Task SendDataAsync(string jsonMessage)
        {
            if (tcpClient.Connected)
            {
                try
                {
                    var bytes = Encoding.UTF8.GetBytes(jsonMessage);
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                    Debug.WriteLine("Data has been sent to the server");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Client {ClientId} cannot send data: {ex}");
                }
            }
            else
            {
                Debug.WriteLine($"Client {ClientId} is not connected to the server.");
            }
        }

        private async Task<string> ReadDataAsync()
        {
            if (tcpClient.Connected)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    var received = await stream.ReadAsync(buffer, 0, buffer.Length);
                    var message = Encoding.UTF8.GetString(buffer, 0, received);
                    Debug.WriteLine($"Client {ClientId} received message");
                    return message;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Client {ClientId} cannot read data from server: {ex}");
                    return null;
                }
            }
            else
            {
                Debug.WriteLine($"Client {ClientId} is not connected to the server.");
                return null;
            }
        }

        public async Task ListenForMessagesAsync()
        {
            if (!tcpClient.Connected)
            {
                Debug.WriteLine($"Client {ClientId} is not connected to the server.");
                return;
            }

            Debug.WriteLine($"Client {ClientId} is waiting for messages.");
            while (tcpClient.Connected)
            {
                var message = await ReadDataAsync();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageReceived?.Invoke(this, message);
                }
            }
        }

        public async Task ListenForUserInfoAsync()
        {
            if (!tcpClient.Connected)
            {
                Debug.WriteLine($"Client {ClientId} is not connected to the server.");
                return;
            }

            Debug.WriteLine($"Client {ClientId} is waiting for user info.");
            while (User == null)
            {
                var message = await ReadDataAsync();
                if (message != null)
                {
                    User = ClientJsonDataSerializer.DeserializeUser(message);
                }
            }
        }

    }
}
