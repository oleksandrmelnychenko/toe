using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Tic_tac_toe_Server.Logging;

namespace Tic_tac_toe_Server.Net
{
    public sealed class RemotePeer(Socket socket, ILogger logger, bool disposed, NetworkStream stream, Action<string> onDataReceived, Action<Guid> onClientDisconnected) : IDisposable
    {
        private const int BufferSize = 8;
        private readonly ArraySegment<byte> _packageBuffer = new ArraySegment<byte>(new byte[BufferSize]);
        private readonly StringBuilder _messageBuffer = new StringBuilder();
        private readonly ILogger _logger = logger;

        public Guid Id { get; set; } = Guid.NewGuid();
        public Socket RemoteEndPoint { get; set; } = socket;
        public NetworkStream Stream { get; init; } = stream;

        private readonly Action<string> _onDataReceived = onDataReceived;
        private readonly Action<Guid> _onClientDisconnected = onClientDisconnected;

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
                    int numberOfBytesRead = RemoteEndPoint.EndReceive(result);
                    if (!IsSocketConnected(RemoteEndPoint))
                    {
                        Dispose();
                        return;
                    }

                    var newSegment = new ArraySegment<byte>(_packageBuffer.Array, _packageBuffer.Offset, numberOfBytesRead);

                    OnDataReceivedAsync(newSegment);
                }

                RemoteEndPoint.BeginReceive(_packageBuffer.Array, _packageBuffer.Offset, _packageBuffer.Count, SocketFlags.None, ReceiveAsyncLoop, null);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError($"ReceiveAsyncLoop argument null exception: {ex.Message}");
            }
            catch(SocketException ex)
            {
                _onClientDisconnected?.Invoke(Id);
                Dispose();
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
            if(RemoteEndPoint.Available == 0)
            {
                _onDataReceived?.Invoke(_messageBuffer.ToString());
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
            if (!disposed)
            {
                if (disposing)
                {
                    RemoteEndPoint.Dispose();
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
