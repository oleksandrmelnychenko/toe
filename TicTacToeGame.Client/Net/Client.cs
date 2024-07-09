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
        private NetworkStream stream;

        private IPEndPoint remoteEndPoint;

        private TcpClient tcpClient;

        public Guid ClientId { get; private set; }

        public event EventHandler<string> MessageReceived;

        public Client(IPAddress address, int port)
        {
            tcpClient = new TcpClient();
            remoteEndPoint = new IPEndPoint(address, port);
        }

        public async Task ConnectAsync()
        {
            try
            {
                await tcpClient.ConnectAsync(remoteEndPoint);
                stream = tcpClient.GetStream();
                Debug.WriteLine($"Client connected to server");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Client cannot connect to server: {ex}");
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

        public async Task ListenForPlayerInfoAsync()
        {
            if (!tcpClient.Connected)
            {
                Debug.WriteLine($"Client is not connected to the server.");
                return;
            }

            Debug.WriteLine($"Client is waiting for player info.");
            while (ClientId == Guid.Empty)
            {
                var message = await ReadDataAsync();
                if (message != null)
                {
                    ClientId = ClientJsonDataSerializer.DeserializePlayer(message).Id;
                    Debug.WriteLine($"Client {ClientId} get client id.");
                }
            }
        }

    }
}
