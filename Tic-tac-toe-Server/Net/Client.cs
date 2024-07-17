using System.Net.Sockets;
using System.Text;

namespace Tic_tac_toe_Server.Net
{
    public class Client : IDisposable
    {
        private bool _disposed;
        public Socket Socket { get; set; }
        public Guid Id { get; private set; }

        private ArraySegment<byte> _buffer;

        public event EventHandler<string> DataReceived = delegate { };

        public Client(Socket socket)
        {
            Id = Guid.NewGuid();
            Socket = socket;
            _buffer = new ArraySegment<byte>(new byte[512]);
        }

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
                        return;
                    }

                    var newSegment = new ArraySegment<byte>(_buffer.Array, _buffer.Offset, numberOfBytesRead);

                    OnDataReceivedAsync(newSegment);
                }

                Socket.BeginReceive(_buffer.Array, _buffer.Offset, _buffer.Count, SocketFlags.None, ReceiveAsyncLoop, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Socket error: {ex.Message}");
            }
        }

        protected virtual void OnDataReceivedAsync(ArraySegment<byte> bytes)
        {
            string receivedText = Encoding.UTF8.GetString(bytes.Array, bytes.Offset, bytes.Count);
            DataReceived?.Invoke(this, receivedText);
        }


        private bool IsSocketConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }

        protected virtual void Dispose(bool disposing)
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
