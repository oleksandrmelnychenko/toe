using Prism.Commands;
using ReactiveUI;
using System.Collections.ObjectModel;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client
{
    public sealed class MainViewModel : ReactiveObject
    {
        private string? _gameStatus;
        private string? _actionHistory;
        private ObservableCollection<BoardCell> _boardBoardCells = null!;
        private readonly GameMaster _gameMaster = new();

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

        public MainViewModel()
        {
            OnCellCommand = new DelegateCommand<BoardCell>(OnCellClickCommandHandler);
            OnRestartCommand = new DelegateCommand(OnRestartClickCommandHandler);

            _gameMaster.StartGame();

            BoardCells = new ObservableCollection<BoardCell>(_gameMaster.GetActiveGameSessionBoard());

            UpdateGameStatus();
        }

        public void OnCellClickCommandHandler(BoardCell boardCell) =>
            ApplyGameAction(boardCell);

        public void OnRestartClickCommandHandler() =>
            RestartGame();

        public void UpdateHistory()
        {
            ActionHistory = _gameMaster.GetHistory();
        }

        //Цим методом просто оновлюю поле для статусу гри, хід юзера змінюю в GameMaster
        public void UpdateGameStatus()
        {
            Status status = _gameMaster.GetStatus();
            string username = _gameMaster.GetCurrentUser().UserSymbolName;

            GameStatus = status switch
            {
                Status.PlayerTurn or Status.Start => GameStatusConst.PlayerTurn + " " + username,
                Status.Draw => GameStatusConst.Draw,
                Status.Finish => GameStatusConst.EndOfGame + " " + username,
                _ => GameStatus
            };
        }

        private void RestartGame()
        {
            _gameMaster.StartGame();
            BoardCells = new ObservableCollection<BoardCell>(_gameMaster.GetActiveGameSessionBoard());
            UpdateGameStatus();
        }

        private void ApplyGameAction(BoardCell boardCell)
        {
            UpdateBoardCell(boardCell);
            _gameMaster.NewAction(boardCell);
            UpdateGameStatus();
            UpdateHistory();
        }

        //Тут напевно неправильна реалізація, не зрозумів як оновлювати клітинку в GameSession так як там IReadOnlyCollection тому просто онвлюю в цьому методі
        private void UpdateBoardCell(BoardCell boardCell)
        {
            boardCell.IsDirty = true;
            boardCell.Value = _gameMaster.GetCurrentUser().UserSymbolName;
            _boardBoardCells[boardCell.Index] = boardCell;
        }
    }
}