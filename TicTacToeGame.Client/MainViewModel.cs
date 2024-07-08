using Prism.Commands;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Tic_tac_toe_Server.Net;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Net;

namespace TicTacToeGame.Client
{
    public sealed class MainViewModel : ReactiveObject
    {
        private string? _gameStatus = GameStatusConst.PlayerTurn + " X";
        private string? _actionHistory;
        private Net.Client client;
        private ushort cellsCount = 9;

        private ObservableCollection<BoardCell> _boardBoardCells = null!;

        private Status gameStatus;

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

        public MainViewModel(Net.Client client)
        {
            OnCellCommand = new DelegateCommand<BoardCell>(OnCellClickCommandHandler);
            OnRestartCommand = new DelegateCommand(OnRestartClickCommandHandler);

            this.client = client;

            BoardCells = new ObservableCollection<BoardCell>(CellFactory.Build(cellsCount));

            this.client.MessageReceived += Client_MessageReceived;
        }

        public async void OnCellClickCommandHandler(BoardCell boardCell) =>
            await ApplyGameAction(boardCell);

        public async void OnRestartClickCommandHandler() =>
            await RestartRequest();

        public void UpdateBoardCell(BoardCell boardCell)
        {
            boardCell.IsDirty = true;
            boardCell.Value = client.Player.PlayerSymbolName;
            _boardBoardCells[boardCell.Index] = boardCell;
        }

        public void UpdateGameData(string message)
        {
            ServerToClientConfig serverMessage = ClientJsonDataSerializer.DeserializeServerMessage(message);
            gameStatus = serverMessage.Status;
            ActionHistory = serverMessage.GameHistory;

            if (serverMessage.CellIndex.HasValue)
            {
                ushort cellIndex = serverMessage.CellIndex.Value;
                Symbol symbol = serverMessage.Symbol ?? Symbol.Empty;

                _boardBoardCells[cellIndex] = new BoardCell(
                    cellIndex,
                    GetSymbolValue(symbol),
                    isDirty: true
                );
            }

            UpdatePlayerData(serverMessage);
        }

        

        public void UpdatePlayerData(ServerToClientConfig serverMessage)
        {
            if (client.Player.Id == serverMessage.CurrentPlayerId)
            {
                client.Player.IsActived = true;
                IsActiveBoard = true;
            }
            else
            {
                client.Player.IsActived = false;
                IsActiveBoard = false;
            }
            UpdateGameStatus();
        }

        public void UpdateGameStatus()
        {
            Status status = gameStatus;
            string symbol = GetCurrentGameSymbol();

            GameStatus = status switch
            {
                Status.PlayerTurn or Status.Start => GameStatusConst.PlayerTurn + " " + symbol,
                Status.Draw => GameStatusConst.Draw,
                Status.Finish => GameStatusConst.EndOfGame + " " + symbol,
                _ => GameStatus
            };
        }

        public string GetCurrentGameSymbol()
        {
            if (client.Player.PlayerSymbolName == SymbolsConst.SymbolX)
            {
                if (client.Player.IsActived)
                {
                    return SymbolsConst.SymbolX;
                }
                else
                {
                    return SymbolsConst.SymbolO;
                }
            }
            else if (client.Player.PlayerSymbolName == SymbolsConst.SymbolO)
            {
                if (client.Player.IsActived)
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

        private string GetSymbolValue(Symbol symbol)
        {
            if (symbol == Symbol.X)
            {
                return Constants.SymbolsConst.SymbolX;
            }
            else if(symbol == Symbol.O)
            {
                return Constants.SymbolsConst.SymbolO;
            }
            else
            {
                return " ";
            }
        }

        private void UpdateCell(ref BoardCell cell, bool isDirty, string value)
        {
            cell = new(cell.Index, value, isDirty);
        }

        private async Task ApplyGameAction(BoardCell boardCell)
        {
            UpdateBoardCell(boardCell);
            ClientToServerConfig clientToServerConfig = new ClientToServerConfig(boardCell.Index, false, client.ClientId);
            string actionJson = ClientJsonDataSerializer.SerializeAction(clientToServerConfig);
            await client.SendDataAsync(actionJson);
        }

        private async Task RestartRequest()
        {
            ClientToServerConfig clientToServerConfig = new ClientToServerConfig(10, true, client.ClientId);
            string requestJson = ClientJsonDataSerializer.SerializeAction(clientToServerConfig);
            await client.SendDataAsync(requestJson);
        }

        private void Client_MessageReceived(object? sender, string message)
        {
            UpdateGameData(message);
        }

    }

}