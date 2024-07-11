using System.Net.Sockets;

namespace Tic_tac_toe_Server.Net
{
    public class Client
    {
        public Socket Socket { get; set; }
        public Guid Id { get; private set; }

        public Client()
        {
            Id = Guid.NewGuid();
        }
    }
}
