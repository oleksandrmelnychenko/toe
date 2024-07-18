using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Player.Factory;

namespace Tic_tac_toe_Server.Player
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

        public void ConnectClientToPlayer(Guid clientId)
        {
            var player = Players.First(p => p.Status == PlayerStatus.Disconnected);

            player.Status = PlayerStatus.Connected;
            player.Id = clientId;
        }

        public void DisconnectClientFromPlayer(Guid clientId)
        {
            var player = Players.First(p => p.Id == clientId);
            player.Status = PlayerStatus.Disconnected;
            player.Id = Guid.Empty;
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
