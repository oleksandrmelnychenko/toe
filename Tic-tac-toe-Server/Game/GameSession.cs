﻿using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net.Messages;
using Tic_tac_toe_Server.Player;
using TicTacToeGame.Client.Net.Messages.ToGameMessages;

namespace Tic_tac_toe_Server.Game
{
    public class GameSession
    {
        private const ushort MaximumPlayerPerRoom = 2;

        private const ushort BoardCellCount = 9;

        private PlayerManager _playerManager;

        private ILogger _logger;

        public bool IsFull { get; private set; } = false;

        public List<BoardCell> BoardCells { get; private set; }

        public Guid Id { get; private set; }

        public Status Status { get; set; } = Status.Start;

        public ActionHistory History { get; set; } = new();

        public event EventHandler<ConfigBase> MessageProcessed = delegate { };

        public GameSession(ILogger logger)
        {
            _logger = logger;
            BoardCells = CellFactory.Build(BoardCellCount);
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
            NewSessionConfig config = new NewSessionConfig(Status, _playerManager.CurrentPlayer.Id, _playerManager.CurrentPlayer.PlayerSymbolName);

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

        public void HandleAction(NewActionMessage action)
        {
            UpdateGameData(action);
            MessageProcessed?.Invoke(this, CreateNewGameDataConfig(action));
        }

        public PlayerManager GetPlayerManager()
        {
            return _playerManager;
        }

        public void Restart()
        {
            BoardCells = CellFactory.Build(BoardCellCount);
            Status = Status.Restart;
            History.ClearHistory();

            MessageProcessed?.Invoke(this, GetStartSessionData());
        }

        private void UpdateGameStatus()
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

        private void HandleGameStatus()
        {
            if (Status == Status.PlayerTurn)
            {
                _playerManager.ChangeCurrentPlayer();
            }
        }
         
        private NewGameDataConfig CreateNewGameDataConfig(NewActionMessage action)
        {
            return new(Status, BoardCells[action.CellIndex].Value, _playerManager.CurrentPlayer.PlayerSymbolName, action.CellIndex, _playerManager.CurrentPlayer.Id, History.History);
        }

        private void UpdateGameData(NewActionMessage action)
        {
            BoardCell cell = new(action.CellIndex, _playerManager.CurrentPlayer.PlayerSymbolName, true);
            BoardCells[cell.Index] = cell;

            GameAction gameAction = new GameAction(_playerManager.GetPlayer(action.ClientId), action.CellIndex);
            History.AddAction(gameAction);

            UpdateGameStatus();
        }
    }
}
