using Prism.Commands;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Net;

namespace TicTacToeGame.Client
{
    public sealed class MainViewModel : ReactiveObject
    {
        private string? _gameStatusField;
        private string? _historyTextField;
        private Net.Client client;

        public string? GameStatusField
        {
            get => _gameStatusField;
            set => this.RaiseAndSetIfChanged(ref _gameStatusField, value);
        }

        public string? HistoryTextField
        {
            get => _historyTextField;
            set => this.RaiseAndSetIfChanged(ref _historyTextField, value);
        }

        private ObservableCollection<BoardCell> _boardBoardCells = null!;

        public ObservableCollection<BoardCell> BoardCells
        {
            get => _boardBoardCells;
            set => this.RaiseAndSetIfChanged(ref _boardBoardCells, value);
        }

        private readonly GameMaster _gameMaster = new();

        public DelegateCommand<BoardCell> OnCellCommand { get; init; }
        public DelegateCommand OnRestartCommand { get; init; }

        public MainViewModel(Net.Client client)
        {
            OnCellCommand = new DelegateCommand<BoardCell>(OnCellClickCommandHandler);
            OnRestartCommand = new DelegateCommand(OnRestartClickCommandHandler);

            this.client = client;
            _gameMaster.StartGame();

            BoardCells = new ObservableCollection<BoardCell>(_gameMaster.GetActiveGameSessionBoard());

            UpdateGameStatusField();

            this.client.MessageReceived += Client_MessageReceived;
        }

        public async void OnCellClickCommandHandler(BoardCell boardCell)
        {
            UpdateBoardCell(boardCell);
            _gameMaster.NewAction(boardCell);
            UpdateGameStatusField();
            UpdateHistoryTextField();

            string boardCellsJson = JsonDataSerializer.SerializeGameData(BoardCells.ToList());
            await client.SendDataAsync(boardCellsJson);
        }

        public void OnRestartClickCommandHandler()
        {
            _gameMaster.StartGame();
            BoardCells = new ObservableCollection<BoardCell>(_gameMaster.GetActiveGameSessionBoard());
            UpdateGameStatusField();
        }

        public void UpdateBoardCell(BoardCell boardCell)
        {
            boardCell.IsDirty = true;
            boardCell.Value = _gameMaster.GetCurrentUser().UserSymbolName;
            _boardBoardCells[boardCell.Index] = boardCell;
        }

        public void UpdateGameData(string message)
        {
            List<BoardCell> boardCells = JsonDataSerializer.DeserializeGameData(message);
            for (int i = 0; i < _boardBoardCells.Count; i++)
            {
                _boardBoardCells[i] = boardCells[i];
            }
        }

        private void Client_MessageReceived(object? sender, string message)
        {
            UpdateGameData(message);
        }

        public void UpdateHistoryTextField()
        {
            HistoryTextField = _gameMaster.GetHistory();
        }

        public void UpdateGameStatusField()
        {
            if (_gameMaster.GetStatus() == Status.PlayerTurn || _gameMaster.GetStatus() == Status.Start)
            {
                GameStatusField = GameStatusConst.PlayerTurn + " " + _gameMaster.GetCurrentUser().UserSymbolName;
            }
            else if (_gameMaster.GetStatus() == Status.Finish)
            {
                GameStatusField = GameStatusConst.EndOfGame + " " + _gameMaster.GetCurrentUser().UserSymbolName;
            }
            else if (_gameMaster.GetStatus() == Status.Draw)
            {
                GameStatusField = GameStatusConst.Draw;
            }
        }
    }

}