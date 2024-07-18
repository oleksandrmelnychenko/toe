using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Net.Messages;

namespace Tic_tac_toe_Server.Net.Strategies
{
    public class RestartStrategy : IMessageStrategy
    {
        public void ProcessMessage(GameMaster master, MessageBase message)
        {
            var restartMessage = message as RestartMessage;
            master.RestartSession(restartMessage!);
        }
    }
}
