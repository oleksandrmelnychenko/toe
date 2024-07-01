using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Tic_tac_toe_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(IPAddress.Parse("127.0.0.1"), 8888);
            server.StartServer();
            //server.ListenClients();
        }
    }
}
