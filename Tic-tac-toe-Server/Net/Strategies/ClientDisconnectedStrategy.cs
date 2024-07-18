using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Net.Messages;

namespace Tic_tac_toe_Server.Net.Strategies
{
    internal class ClientDisconnectedStrategy : IMessageStrategy
    {
        public void ProcessMessage(GameMaster master, MessageBase message)
        {
            var clientDisconnectedMessage = message as ClientDisconnectedMessage;
            master.ClientDisconnected(clientDisconnectedMessage!.ClientId);
        }
    }
}
