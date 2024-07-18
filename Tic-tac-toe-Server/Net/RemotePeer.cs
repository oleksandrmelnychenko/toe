using System.Net.Sockets;
using System.Text;
using Tic_tac_toe_Server.Logging;

namespace Tic_tac_toe_Server.Net
{
    public sealed class RemotePeer(Socket socket, ILogger logger) : IDisposable
    {
        private const int BufferSize = 8;
        private bool _disposed;
        private ArraySegment<byte> _buffer = new ArraySegment<byte>(new byte[BufferSize]);
        private StringBuilder _messageBuffer = new StringBuilder();
        private ILogger _logger = logger;

        public Socket Socket { get; set; } = socket;
        public Guid Id { get; private set; } = Guid.NewGuid();
        public event EventHandler<string> DataReceived = delegate { };

        public void StartReceiveAsync()
        {
            ReceiveAsyncLoop(null);
        }

        private void ReceiveAsyncLoop(IAsyncResult result)
        {
            try
            {
                if (result != null)
                {
                    int numberOfBytesRead = Socket.EndReceive(result);
                    if (!IsSocketConnected(Socket))
                    {
                        Dispose();
                        return;
                    }

                    var newSegment = new ArraySegment<byte>(_buffer.Array, _buffer.Offset, numberOfBytesRead);

                    OnDataReceivedAsync(newSegment);
                }

                Socket.BeginReceive(_buffer.Array, _buffer.Offset, _buffer.Count, SocketFlags.None, ReceiveAsyncLoop, null);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError($"ReceiveAsyncLoop argument null exception: {ex.Message}");
            }
            catch(SocketException ex)
            {
                _logger.LogError($"ReceiveAsyncLoop socket exception: {ex.Message}");
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogError($"ReceiveAsyncLoop object disposed exception: {ex.Message}");
            }
            catch(ArgumentOutOfRangeException ex)
            {
                _logger.LogError($"ReceiveAsyncLoop argument out of range exception: {ex.Message}");
            }
            catch(Exception ex)
            {
                _logger.LogError($"ReceiveAsyncLoop unexpected exception: {ex.Message}");
            }
        }

        private void OnDataReceivedAsync(ArraySegment<byte> bytes)
        {
            _messageBuffer.Append(Encoding.UTF8.GetString(bytes.Array, bytes.Offset, bytes.Count));
            if(Socket.Available == 0)
            {
                DataReceived?.Invoke(this, _messageBuffer.ToString());
                _messageBuffer.Clear();
            }
        }


        private bool IsSocketConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Socket.Dispose();
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
