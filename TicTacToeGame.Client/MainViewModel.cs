using Prism.Commands;
using ReactiveUI;
using System.Collections.ObjectModel;
using Tic_tac_toe_Server.Net;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Net;

namespace TicTacToeGame.Client
{
    public sealed class MainViewModel : ReactiveObject
    {
        private string? _gameStatusField = GameStatusConst.PlayerTurn + " X";
        private string? _historyTextField;
        private Net.Client client;
        private ushort cellsCount = 9;
        public bool IsActive { get; set; } = false;

        private Status gameStatus;

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

            BoardCells = new ObservableCollection<BoardCell>(CellFactory.Build(cellsCount));

            this.client.MessageReceived += Client_MessageReceived;
        }

        public async void OnCellClickCommandHandler(BoardCell boardCell)
        {
            UpdateBoardCell(boardCell);
            ClientGameMessage playerMove = new ClientGameMessage(client.ClientId, boardCell);
            string moveJson = ClientJsonDataSerializer.SerializeMove(playerMove);
            await client.SendDataAsync(moveJson);
        }

        public void OnRestartClickCommandHandler()
        {
            BoardCells = new ObservableCollection<BoardCell>(CellFactory.Build(cellsCount));
            UpdateGameStatusField();
        }

        public void UpdateBoardCell(BoardCell boardCell)
        {
            boardCell.IsDirty = true;
            boardCell.Value = client.User.UserSymbolName;
            _boardBoardCells[boardCell.Index] = boardCell;
        }

        public void UpdateGameData(string message)
        {
            ServerGameMessage serverMessage = ClientJsonDataSerializer.DeserializeServerMessage(message);
            gameStatus = serverMessage.GameStatus;
            HistoryTextField = serverMessage.GameHistory;
            for (int i = 0; i < _boardBoardCells.Count; i++)
            {
                _boardBoardCells[i] = serverMessage.BoardCells[i];
            }

            UpdateUserData(serverMessage);
        }

        public void UpdateUserData(ServerGameMessage serverMessage)
        {
            if (client.User.Id == serverMessage.User.Id)
            {
                client.User.IsActived = true;
            }
            else
            {
                client.User.IsActived = false;
            }

            UpdateGameStatusField();
        }

        private void Client_MessageReceived(object? sender, string message)
        {
            UpdateGameData(message);
        }

        public void UpdateGameStatusField()
        {
            if (gameStatus == Status.PlayerTurn || gameStatus == Status.Start)
            {
                GameStatusField = GameStatusConst.PlayerTurn + " " + GetCurrentGameSymbol();
            }
            else if (gameStatus == Status.Finish)
            {
                GameStatusField = GameStatusConst.EndOfGame + " " + client.User.UserSymbolName;
            }
            else if (gameStatus == Status.Draw)
            {
                GameStatusField = GameStatusConst.Draw;
            }
        }

        public string GetCurrentGameSymbol()
        {
            if (client.User.UserSymbolName == SymbolsConst.SymbolX)
            {
                if (client.User.IsActived)
                {
                    return SymbolsConst.SymbolX;
                }
                else
                {
                    return SymbolsConst.SymbolO;
                }
            }
            else if (client.User.UserSymbolName == SymbolsConst.SymbolO)
            {
                if (client.User.IsActived)
                {
                    return SymbolsConst.SymbolO;
                }
                else
                {
                    return SymbolsConst.SymbolX;
                }
            }
            else
            {
                return "unknown player!";
            }
        }
    }

}