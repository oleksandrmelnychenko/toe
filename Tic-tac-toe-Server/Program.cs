using System.Net.Sockets;
using System.Net;
using System.Text;
using Tic_tac_toe_Server.Logging;

namespace Tic_tac_toe_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleLogger consoleLogger = new ConsoleLogger();
            TicTacToeServer.Server server = new TicTacToeServer.Server(IPAddress.Parse("127.0.0.1"), 8888, consoleLogger);
            server.StartServerAsync().GetAwaiter().GetResult();
            while (server.IsActive == true)
            {
                server.ListenClientsAsync().GetAwaiter().GetResult();
            }
        }
    }
}
