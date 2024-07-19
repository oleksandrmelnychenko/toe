using Tic_tac_toe_Server.Net.Strategies;
using TicTacToeGame.Client.Net.Messages;
using Tmds.DBus.Protocol;

namespace TicTacToeGame.Client.Net.Strategies
{
    internal class ClientInitializationStrategy : IMessageStrategy
    {
        public void ProcessMessage(MainViewModel viewModel, MessageBase message)
        {
            var initializeMessage = message as ClientInitializationMessage;
            viewModel.InitializeClient(initializeMessage.PlayerId);
        }
    }
}
