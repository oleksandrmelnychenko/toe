using System.Net;
using Tic_tac_toe_Server.Constants;
using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net;

namespace Tic_tac_toe_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleLogger consoleLogger = new ConsoleLogger();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(NetworkAddressConfig.IPAddress), NetworkAddressConfig.Port);

            Server server = new(endPoint, consoleLogger);
            GameMaster gameMaster = new(consoleLogger);
            server.StartServer();

            DataTransferManager dataTransferManager = new DataTransferManager(server, gameMaster, consoleLogger);

            while (true)
            {

            }
        }
    }
}
