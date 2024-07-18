using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Net.Messages;

namespace Tic_tac_toe_Server.Net.Strategies
{
    public interface IMessageStrategy
    {
        public void ProcessMessage(GameMaster master, MessageBase message);
    }
}
