using Tic_tac_toe_Server.Game.Interfaces;
using TicTacToeGame.Client.Models;

namespace TicTacToeGame.Client.Game
{
    public class PlayerManager
    {
        private int _currentPlayerIndex;
        private IPlayerInitializator _playerInitializator;

        private Player[] _players;

        public Player[] Players
        {
            get => _players;
            private set => _players = value;
        }

        public Player CurrentPlayer => _players[_currentPlayerIndex];

        public PlayerManager(IPlayerInitializator playerInitializator)
        {
            _playerInitializator = playerInitializator;
            _players = _playerInitializator.InitializePlayers();
        }

        public void ChangeCurrentPlayer()
        {
            _currentPlayerIndex = _currentPlayerIndex == 0 ? 1 : 0;
        }
    }
}
