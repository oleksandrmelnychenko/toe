using Tic_tac_toe_Server.Net;
using Tic_tac_toe_Server.Player;
using Tic_tac_toe_Server.Player.Factory;
using TicTacToeGame.Client.Net;

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

        public void ConnectClientToPlayer(ref Net.Client client)
        {
            var player = Players.FirstOrDefault(p => p.Status == PlayerStatus.Disconnected);
            if (player != null)
            {
                player.Status = PlayerStatus.Connected;
                client.Id = player.Id;
            }
        }

        public void DisconnectClientToPlayer(Guid id)
        {
            var player = Players.FirstOrDefault(p => p.Id == id);
            if (player != null)
            {
                player.Status = PlayerStatus.Disconnected;
            }
        }
    }
}
