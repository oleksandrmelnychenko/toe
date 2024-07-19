using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Net.Configs;
using TicTacToeGame.Client.Net.Messages;

namespace TicTacToeGame.Client.Net
{
    public class Client : IDisposable
    {
        private NetworkStream _stream;

        private bool _disposed;

        private Socket _tcpClient = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private const int BufferSize = 128;

        private ArraySegment<byte> _buffer = new ArraySegment<byte>(new byte[BufferSize]);

        private StringBuilder _messageBuffer = new StringBuilder();

        public Guid ClientId { get; private set; }

        public event Action<string> MessageReceived;

        public async Task ConnectAsync(IPEndPoint endPoint)
        {
            try
            {
                await _tcpClient.ConnectAsync(endPoint);
                _stream = new NetworkStream(_tcpClient);
                Debug.WriteLine($"Client connected to server");

                _ = Task.Run(() => StartReceive());
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

        private void StartReceive()
        {
            ReceiveAsyncLoop(null);
        }

        private void ReceiveAsyncLoop(IAsyncResult result)
        {
            try
            {
                if (result != null)
                {
                    int numberOfBytesRead = _tcpClient.EndReceive(result);
                    if (numberOfBytesRead == 0)
                    {
                        this.Dispose();
                        return;
                    }

                    var newSegment = new ArraySegment<byte>(_buffer.Array, _buffer.Offset, numberOfBytesRead);

                    OnDataReceivedAsync(newSegment);
                }

                _tcpClient.BeginReceive(_buffer.Array, _buffer.Offset, _buffer.Count, SocketFlags.None, ReceiveAsyncLoop, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Socket error: {ex.Message}");
            }
        }

        private void OnDataReceivedAsync(ArraySegment<byte> newSegment)
        {
            _messageBuffer.Append(Encoding.UTF8.GetString(newSegment.Array, newSegment.Offset, newSegment.Count));

            if(_tcpClient.Available == 0)
            {
                MessageReceived?.Invoke(_messageBuffer.ToString());
                _messageBuffer.Clear();
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
