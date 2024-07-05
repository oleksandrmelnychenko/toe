using TicTacToeGame.Client.Models;

namespace Tic_tac_toe_Server.Game.Interfaces
{
    public interface IPlayerInitializator
    {
        public Player[] InitializePlayers();
    }
}
