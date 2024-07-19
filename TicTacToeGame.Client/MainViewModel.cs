using Prism.Commands;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Tic_tac_toe_Server.Net.Strategies;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Net;
using TicTacToeGame.Client.Net.Configs;
using TicTacToeGame.Client.Net.Messages;

namespace TicTacToeGame.Client
{
    public sealed class MainViewModel : ReactiveObject
    {
        private string? _gameStatus = GameStatusConst.PlayerTurn + " X";
        private string? _actionHistory;
        private ushort cellsCount = 9;
        private Guid _playerId = Guid.Empty;
        private IMessageStrategy _messageStrategy;

        private ObservableCollection<BoardCell> _boardBoardCells = null!;

        private bool isActiveBoard;

        public bool IsActiveBoard
        {
            get => isActiveBoard;
            set => this.RaiseAndSetIfChanged(ref isActiveBoard, value);
        }

        public string? GameStatus
        {
            get => _gameStatus;
            set => this.RaiseAndSetIfChanged(ref _gameStatus, value);
        }

        public string? ActionHistory
        {
            get => _actionHistory;
            set => this.RaiseAndSetIfChanged(ref _actionHistory, value);
        }

        public ObservableCollection<BoardCell> BoardCells
        {
            get => _boardBoardCells;
            set => this.RaiseAndSetIfChanged(ref _boardBoardCells, value);
        }

        public DelegateCommand<BoardCell> OnCellCommand { get; init; }
        public DelegateCommand OnRestartCommand { get; init; }

        public event Action<ConfigBase> SubmitData = delegate { };

        public MainViewModel()
        {
            OnCellCommand = new DelegateCommand<BoardCell>(OnCellClickCommandHandler);
            OnRestartCommand = new DelegateCommand(OnRestartClickCommandHandler);

            BoardCells = new ObservableCollection<BoardCell>(CellFactory.Build(cellsCount));
        }

        public async void OnCellClickCommandHandler(BoardCell boardCell) =>
            await ApplyGameAction(boardCell);

        public async void OnRestartClickCommandHandler() =>
            await RestartRequest();

        public void SetStrategy(IMessageStrategy strategy)
        {
            _messageStrategy = strategy;
        }

        public void UpdateGameData(NewGameDataMessage serverMessage)
        {
            if (serverMessage == null)
            {
                Debug.WriteLine("Server send ivalid data!!!");
                return;
            }

            UpdateActionHistory(serverMessage.ActionHistory);

            UpdateBoardCell(serverMessage.CellIndex, serverMessage.CellSymbol);

            UpdatePlayerData(serverMessage.CurrentPlayerId, serverMessage.Status);
            UpdateGameStatus(serverMessage.Status, serverMessage.CurrentPlayerSymbol);
        }

        public void NewGameSession(NewGameSessionMessage serverMessage)
        {
            if (serverMessage.Status == Status.Restart) Restart();

            if (serverMessage == null)
            {
                Debug.WriteLine("Server send ivalid data!!!");
                return;
            }

            UpdateGameStatus(serverMessage.Status, serverMessage.CurrentPlayerSymbol);
            UpdatePlayerData(serverMessage.CurrentPlayerId, serverMessage.Status);
        }

        public void InitializeClient(Guid id)
        {
            _playerId = id;
            PlayerInitializedConfig config = new(id);
            SubmitData?.Invoke(config);
        }

        private void UpdateActionHistory(string history)
        {
            ActionHistory = history;
        }

        private void UpdateBoardCell(ushort cellIndex, Symbol? cellSymbol)
        {
            Symbol symbol = cellSymbol ?? Symbol.Empty;

            _boardBoardCells[cellIndex] = new BoardCell(
                cellIndex,
                symbol,
                isDirty: true
            );
        }


        private void Restart()
        {
            BoardCells = new ObservableCollection<BoardCell>(CellFactory.Build(cellsCount));
        }

        private void UpdatePlayerData(Guid id, Status status)
            => IsActiveBoard = _playerId == id && (status != Status.Finish && status != Status.Draw) ? true : false;

        private void UpdateGameStatus(Status status, Symbol symbol)
        {
            GameStatus = status switch
            {
                Status.PlayerTurn or Status.Start or Status.Restart => GameStatusConst.PlayerTurn + " " + symbol,
                Status.Draw => GameStatusConst.Draw,
                Status.Finish => GameStatusConst.EndOfGame + " " + symbol,
                _ => GameStatus
            };
        }

        private async Task ApplyGameAction(BoardCell boardCell)
        {
            NewActionConfig newActionConfig = new(boardCell.Index, _playerId);

            SubmitData?.Invoke(newActionConfig);
        }

        private async Task RestartRequest()
        {
            RestartConfig restartConfig = new(_playerId);
            SubmitData?.Invoke(restartConfig);
        }

        public void OnMessageRecived(MessageBase message)
        {
            _messageStrategy.ProcessMessage(this, message);
        }
    }
}