using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Net.Messages;

namespace Tic_tac_toe_Server.Net.Strategies
{
    public class NewActionStrategy : IMessageStrategy
    {
        public void ProcessMessage(GameMaster master, MessageBase message)
        {
            var actionMessage = message as NewActionMessage;
            master.NewAction(actionMessage!);
        }
    }
}
