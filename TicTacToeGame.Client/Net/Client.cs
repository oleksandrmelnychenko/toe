using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeGame.Client.Net
{
    public class Client : IDisposable
    {
        private NetworkStream _stream;

        private bool _disposed;

        private IPEndPoint _remoteEndPoint;

        private TcpClient _tcpClient;

        public Guid ClientId { get; private set; }

        public event EventHandler<string> MessageReceived;

        public Client(IPAddress address, int port)
        {
            _tcpClient = new TcpClient();
            _remoteEndPoint = new IPEndPoint(address, port);
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _tcpClient.ConnectAsync(_remoteEndPoint);
                _stream = _tcpClient.GetStream();
                Debug.WriteLine($"Client connected to server");
                await ListenForPlayerInfoAsync();
                await SendDataAsync("Hello");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Client cannot connect to server: {ex}");
            }
        }

        public async Task SendDataAsync(string jsonMessage)
        {
            if (_tcpClient.Connected)
            {
                try
                {
                    var bytes = Encoding.UTF8.GetBytes(jsonMessage);
                    await _stream.WriteAsync(bytes, 0, bytes.Length);
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
            if (_tcpClient.Connected)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    var received = await _stream.ReadAsync(buffer, 0, buffer.Length);
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
            if (!_tcpClient.Connected)
            {
                Debug.WriteLine($"Client {ClientId} is not connected to the server.");
                return;
            }

            Debug.WriteLine($"Client {ClientId} is waiting for messages.");
            while (_tcpClient.Connected)
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
            if (!_tcpClient.Connected)
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
                    ClientId = ClientJsonDataSerializer.DeserializePlayerId(message);
                    Debug.WriteLine($"Client {ClientId} get client id.");
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
                if(disposing)
                {
                    if(_stream.CanRead)
                    {
                        _stream.Close();
                        _stream.Dispose();
                    }

                    if(_tcpClient.Connected)
                    {
                        _tcpClient.Close();
                        _tcpClient.Dispose();
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
