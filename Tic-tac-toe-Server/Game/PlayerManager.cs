using Tic_tac_toe_Server.Net;
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
            set => _players = value;
        }

        public PlayerBase CurrentPlayer => _players[_currentPlayerIndex];

        public PlayerManager(ushort count)
        {
            _players = PlayerFactory.Build(count);
        }

        public void ChangeCurrentPlayer()
        {
            _currentPlayerIndex = _currentPlayerIndex == 0 ? 1 : 0;
        }

        public bool ConnectClientToPlayer(Client client)
        {
            if(!IsSessionFull())
            {
                var player = Players.FirstOrDefault(p => p.Status == PlayerStatus.Disconnected);
                player.Status = PlayerStatus.Connected;
                player.Id = client.Id;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsSessionFull()
        {
            return !Players.Any(p => p.Status == PlayerStatus.Disconnected);
        }
    }
}
