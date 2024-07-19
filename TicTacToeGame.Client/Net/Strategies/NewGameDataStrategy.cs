using Tic_tac_toe_Server.Net.Strategies;
using TicTacToeGame.Client.Net.Messages;

namespace TicTacToeGame.Client.Net.Strategies
{
    internal class NewGameDataStrategy : IMessageStrategy
    {
        public void ProcessMessage(MainViewModel viewModel, MessageBase message)
        {
            var newGameDataMessage = message as NewGameDataMessage;
            viewModel.UpdateGameData(newGameDataMessage);
        }
    }
}
