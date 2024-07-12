using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net;
using Tic_tac_toe_Server.Player;
using TicTacToeGame.Client.Game;

namespace Tic_tac_toe_Server.Game
{
    public class GameSession
    {
        private const ushort MaximumPlayerPerRoom = 2;

        private PlayerManager _playerManager;

        private ILogger _logger;

        public bool IsFull { get; private set; } = false;

        public List<BoardCell> BoardCells { get; private set; }

        public Guid Id { get; private set; }

        public Status Status { get; set; } = Status.Start;

        public ActionHistory History { get; set; } = new();

        public GameSession(ILogger logger)
        {
            _logger = logger;
            _playerManager = new(MaximumPlayerPerRoom, logger);
            BoardCells = CellFactory.Build(9);
        }

        public bool AddPlayer(Guid clientId)
        {
            if (_playerManager.HasFreeSlots())
            {
                _playerManager.ConnectClientToPlayer(clientId);

                IsFull = !_playerManager.HasFreeSlots();
                return true;
            }
            else
            {
                _logger.LogWarning($"Session: {this.Id} full.");
                return false;
            }
        }

        public void HandleAction(GameAction action)
        {
            History.AddAction(action);
            UpdateGameStatus();
        }

        public PlayerBase GetCurrentPlayer()
        {
            return _playerManager.CurrentPlayer;
        }

        public void UpdateGameStatus()
        {
            Status = OutcomeDeterminer.IsWinner(BoardCells) switch
            {
                true => Status.Finish,
                false => OutcomeDeterminer.IsDraw(BoardCells) switch
                {
                    true => Status.Draw,
                    false => Status.PlayerTurn
                }
            };

            HandleGameStatus();
        }

        public PlayerManager GetPlayerManager()
        {
            return _playerManager;
        }

        private void HandleGameStatus()
        {
            if (Status == Status.PlayerTurn)
            {
                _playerManager.ChangeCurrentPlayer();
            }
        }
    }
}
