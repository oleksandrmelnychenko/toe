using System.Net.Sockets;
using System.Text;

namespace Tic_tac_toe_Server.Net
{
    public class Client
    {
        public Socket Socket { get; set; }
        public Guid Id { get; private set; }

        private ArraySegment<byte> _buffer;

        public event EventHandler<string> DataReceived;

        public Client(Socket socket)
        {
            Id = Guid.NewGuid();
            Socket = socket;
            _buffer = new ArraySegment<byte>(new byte[256]);
        }

        public async Task StartReceiveAsync()
        {
            await ReceiveAsyncLoop();
        }

        private async Task ReceiveAsyncLoop()
        {
            try
            {
                while (true)
                {
                    int numberOfBytesRead = await Socket.ReceiveAsync(_buffer, SocketFlags.None);
                    if (numberOfBytesRead == 0)
                    {
                        Socket.Dispose();
                        return;
                    }

                    var receivedData = new ArraySegment<byte>(_buffer.Array, _buffer.Offset, numberOfBytesRead);
                    await OnDataReceivedAsync(receivedData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Socket error: {ex.Message}");
            }
        }

        protected virtual async Task OnDataReceivedAsync(ArraySegment<byte> bytes)
        {
            string receivedText = Encoding.UTF8.GetString(bytes.Array, bytes.Offset, bytes.Count);
            DataReceived?.Invoke(this, receivedText);
        }
    }
}
