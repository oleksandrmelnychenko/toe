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

        public void UpdateGameData(string message)
        {
            ServerToClientConfig serverMessage = ClientJsonDataSerializer.DeserializeServerMessage(message);
            ActionHistory = serverMessage.GameHistory;

            if(serverMessage.Status == Status.Restart)
            {
                Restart();
            }

            if (serverMessage.CellIndex.HasValue)
            {
                ushort cellIndex = serverMessage.CellIndex.Value;
                Symbol symbol = serverMessage.CellSymbol ?? Symbol.Empty;

                _boardBoardCells[cellIndex] = new BoardCell(
                    cellIndex,
                    symbol,
                    isDirty: true
                );
            }

            UpdatePlayerData(serverMessage);

            UpdateGameStatus(serverMessage.Status, serverMessage.CurrentPlayerSymbol);
        }

        private void Restart()
        {
            BoardCells = new ObservableCollection<BoardCell>(CellFactory.Build(cellsCount));
        }

        private void UpdatePlayerData(ServerToClientConfig serverMessage)
            => IsActiveBoard = client.ClientId == serverMessage.CurrentPlayerId ? true : false;

        private void UpdateGameStatus(Status status, Symbol symbol)
        {
            GameStatus = status switch
            {
                Status.PlayerTurn or Status.Start => GameStatusConst.PlayerTurn + " " + symbol,
                Status.Draw => GameStatusConst.Draw,
                Status.Finish => GameStatusConst.EndOfGame + " " + symbol,
                _ => GameStatus
            };
        }

        private async Task ApplyGameAction(BoardCell boardCell)
        {
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