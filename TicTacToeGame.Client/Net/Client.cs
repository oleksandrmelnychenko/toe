using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeGame.Client.Net
{
    public class Client
    {
        private TcpClient tcpClient;
        public Guid Id { get; init; }

        private NetworkStream stream;

        private IPEndPoint remoteEndPoint;

        public Client(IPAddress address, int port)
        {
            tcpClient = new TcpClient();
            Id = Guid.NewGuid();
            remoteEndPoint = new IPEndPoint(address, port);
        }

        public async Task ConnectAsync()
        {
            try
            {
                await tcpClient.ConnectAsync(remoteEndPoint);
                stream = tcpClient.GetStream();
                Debug.WriteLine($"Client {Id} connect to server");
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Client {Id} can not connect to server: {ex}");
            }
        }

        public async Task SendDataAsync()
        {
            if (tcpClient.Connected)
            {
                try
                {
                    var message = $"Client {Id} sent data: {DateTime.Now}";
                    var bytes = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(bytes, 0, bytes.Length);

                    Debug.WriteLine("Data has been sent to the server");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Client {Id} can not send data: {ex}");
                }
            }
            else
            {
                Debug.WriteLine($"Client {Id} is not connected to the server.");
            }
        }
    }
}
