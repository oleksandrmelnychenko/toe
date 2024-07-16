using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Net.Messages;
using TicTacToeGame.Client.Net.Messages.ToClientMessages;
using Tmds.DBus.Protocol;

namespace TicTacToeGame.Client.Net
{
    public class Client : IDisposable
    {
        private NetworkStream _stream;

        private bool _disposed;

        private IPEndPoint _remoteEndPoint;

        private Socket _tcpClient;

        private ArraySegment<byte> _buffer;

        public bool IsInitialized { get; private set; } = false;

        public Guid ClientId { get; private set; }

        public event EventHandler<MessageBase> MessageReceived;

        public Client(IPEndPoint endPoint)
        {

            _buffer = new ArraySegment<byte>(new byte[256]);

            _tcpClient = new Socket(SocketType.Stream, ProtocolType.Tcp);

            SetEndPoint(endPoint);
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _tcpClient.ConnectAsync(_remoteEndPoint);
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
            string receivedText = Encoding.UTF8.GetString(newSegment.Array, newSegment.Offset, newSegment.Count);

            MessageBase message = Serializer.ParseMessage(receivedText);
            if (!IsInitialized)
            {
                ClientInitialization(message);
            }
            else
            {
                MessageReceived?.Invoke(this, message);
            }
        }

        private void ClientInitialization(MessageBase message)
        {
            if (message is ClientInitializationMessage initMessage)
            {
                ClientId = initMessage.PlayerId;
                IsInitialized = true;
                Debug.WriteLine($"Client initialized with ID {ClientId}");
            }
            else
            {
                Debug.WriteLine("Received non-initialization message before initialization.");
                return;
            }
        }

        private void SetEndPoint(IPEndPoint endPoint)
        {
            (bool isValidEndPoint, string endPointValidationMessage) = EndPointValidation.IsValidEndPoint(endPoint);
            if (isValidEndPoint)
            {
                _remoteEndPoint = endPoint;
            }
            else
            {
                Debug.WriteLine($"Remote end point is not valid!");
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
