using System.Net.Sockets;
using System.Net;
using System.Text;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleLogger consoleLogger = new ConsoleLogger();
            MainGame game = new MainGame(consoleLogger);
            game.Start().GetAwaiter().GetResult();
        }
    }
}
