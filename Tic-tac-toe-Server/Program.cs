using System.Net.Sockets;
using System.Net;
using System.Text;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Net;
using Tic_tac_toe_Server.Constants;

namespace Tic_tac_toe_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleLogger consoleLogger = new ConsoleLogger();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(AddressConstants.IPAddress), AddressConstants.Port);

            Server server = new(endPoint, consoleLogger);
            GameMaster gameMaster = new(server, consoleLogger);

            while(true)
            {

            }
        }
    }
}
