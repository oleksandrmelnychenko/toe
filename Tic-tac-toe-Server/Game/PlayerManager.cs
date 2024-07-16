using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net;
using Tic_tac_toe_Server.Player;
using Tic_tac_toe_Server.Player.Factory;

namespace Tic_tac_toe_Server.Game
{
    public class PlayerManager
    {
        private int _currentPlayerIndex;

        private List<PlayerBase> _players;

        private ILogger _logger;

        public List<PlayerBase> Players
        {
            get => _players;
            set => _players = value;
        }

        public PlayerBase CurrentPlayer => _players[_currentPlayerIndex];

        public PlayerManager(ushort count, ILogger logger)
        {
            _logger = logger;
            _players = PlayerFactory.Build(count);
        }

        public void ChangeCurrentPlayer()
        {
            _currentPlayerIndex = _currentPlayerIndex == 0 ? 1 : 0;
        }

        public bool ConnectClientToPlayer(Guid clientId)
        {
            var player = Players.FirstOrDefault(p => p.Status == PlayerStatus.Disconnected);

            if (player == null)
            {
                _logger.LogError("Can not connect client to player.");
                return false;
            }

            player.Status = PlayerStatus.Connected;
            player.Id = clientId;
            return true;
        }

        public bool HasPlayer(Guid playerId)
        {
            return _players.Any(p => p.Id == playerId);
        }

        public PlayerBase GetPlayer(Guid playerId)
        {
            return _players.FirstOrDefault(p => p.Id == playerId);
        }

        public bool HasFreeSlots()
        {
            return Players.Any(p => p.Status == PlayerStatus.Disconnected);
        }
    }
}
