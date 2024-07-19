using Tic_tac_toe_Server.Net.Strategies;
using TicTacToeGame.Client.Net.Messages;

namespace TicTacToeGame.Client.Net.Strategies
{
    internal class NewGameSessionStrategy : IMessageStrategy
    {
        public void ProcessMessage(MainViewModel viewModel, MessageBase message)
        {
            var newGameSessionMessage = message as NewGameSessionMessage;
            viewModel.NewGameSession(newGameSessionMessage!);
        }
    }
}
