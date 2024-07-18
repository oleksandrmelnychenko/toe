using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Net.Messages;

namespace Tic_tac_toe_Server.Net.Strategies
{
    public class PlayerInitializedStrategy : IMessageStrategy
    {
        public void ProcessMessage(GameMaster master, MessageBase message)
        {
            var initializeMessage = message as PlayerInitializedMessage;
            master.ClientConnected(initializeMessage!.ClientId);
        }
    }
}
