using Tic_tac_toe_Server.Player;
using Tic_tac_toe_Server.Player.Factory;

namespace Tic_tac_toe_Server.Game
{
    public class PlayerManager
    {
        private int _currentPlayerIndex;

        private List<PlayerBase> _players;

        public List<PlayerBase> Players
        {
            get => _players;
            private set => _players = value;
        }

        public PlayerBase CurrentPlayer => _players[_currentPlayerIndex];

        public PlayerManager(int count)
        {
            _players = PlayerFactory.CreatePlayers(count);
        }

        public void ChangeCurrentPlayer()
        {
            _currentPlayerIndex = _currentPlayerIndex == 0 ? 1 : 0;
        }
    }
}
