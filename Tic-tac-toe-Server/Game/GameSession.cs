using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Messages;
using Tic_tac_toe_Server.Player;

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
            BoardCells = CellFactory.Build(9);
            _playerManager = new(MaximumPlayerPerRoom, logger);
        }

        public void AddPlayer(Guid clientId)
        {
            if (_playerManager.HasFreeSlots())
            {
                _playerManager.ConnectClientToPlayer(clientId);

                IsFull = !_playerManager.HasFreeSlots();
            }
            else
            {
                _logger.LogWarning($"Session: {this.Id} full.");
            }
        }

        /// <summary>
        /// Sends the initial game data to players when the session is full.
        /// </summary>
        /// <returns>A NewSessionConfig containing the initial game data.</returns>
        public NewSessionConfig GetStartSessionData()
        {
            NewSessionConfig config = new NewSessionConfig(Status, GetCurrentBoardSymbols(), _playerManager.CurrentPlayer.Id);

            return config;
        }

        public List<Guid> GetSessionPlayers()
        {
            List<Guid> playersIds = new List<Guid>();

            foreach (var player in _playerManager.Players)
            {
                playersIds.Add(player.Id);
            }

            return playersIds;
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

        private List<Symbol> GetCurrentBoardSymbols()
        {
            List<Symbol> symbols = new List<Symbol>(BoardCells.Count);
            foreach (BoardCell boardCell in BoardCells)
            {
                symbols.Add(boardCell.Value);
            }

            return symbols;
        }
    }
}
