using TicTacToeGame.Client;
using TicTacToeGame.Client.Net.Messages;

namespace Tic_tac_toe_Server.Net.Strategies
{
    public interface IMessageStrategy
    {
        public void ProcessMessage(MainViewModel viewModel, MessageBase message);
    }
}
