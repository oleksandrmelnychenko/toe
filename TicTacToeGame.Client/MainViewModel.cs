using Prism.Commands;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Net;
using TicTacToeGame.Client.Net.Configs;
using TicTacToeGame.Client.Net.Messages;
using TicTacToeGame.Client.Net.Messages.ToGameMessages;

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

        public MainViewModel()
        {
            InitializeClient();

            OnCellCommand = new DelegateCommand<BoardCell>(OnCellClickCommandHandler);
            OnRestartCommand = new DelegateCommand(OnRestartClickCommandHandler);

            BoardCells = new ObservableCollection<BoardCell>(CellFactory.Build(cellsCount));

            this.client.MessageReceived += Client_MessageReceived;
        }

        public async void OnCellClickCommandHandler(BoardCell boardCell) =>
            await ApplyGameAction(boardCell);

        public async void OnRestartClickCommandHandler() =>
            await RestartRequest();

        public void UpdateGameData(NewGameDataMessage serverMessage)
        {
            if (serverMessage == null)
            {
                Debug.WriteLine("Server send ivalid data!!!");
                return;
            }


            UpdateActionHistory(serverMessage.GameHistory);
            HandleRestartStatus(serverMessage.Status);

            //Перевірку зробити
            UpdateBoardCell(serverMessage.CellIndex, serverMessage.CellSymbol);

            UpdatePlayerData(serverMessage.CurrentPlayerId);
            UpdateGameStatus(serverMessage.Status, serverMessage.CurrentPlayerSymbol);
        }

        public void NewGameSession(NewGameSessionMessage serverMessage)
        {
            if (serverMessage == null)
            {
                Debug.WriteLine("Server send ivalid data!!!");
                return;
            }

            UpdatePlayerData(serverMessage.CurrentPlayerId);
            UpdateGameStatus(serverMessage.Status, serverMessage.CurrentPlayerSymbol);
        }

        private void UpdateActionHistory(string history)
        {
            ActionHistory = history;
        }

        private void HandleRestartStatus(Status status)
        {
            if (status == Status.Restart)
            {
                Restart();
            }
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

        private void UpdatePlayerData(Guid id)
            => IsActiveBoard = client.ClientId == id ? true : false;

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
            NewActionConfig newActionConfig = new(boardCell.Index, client.ClientId);
            JsonValidationResult actionJson = Serializer.SerializeNewAction(newActionConfig);
            if (actionJson.IsValid)
            {
                await client.SendDataAsync(actionJson.JsonMessage);
            }
        }

        private async Task RestartRequest()
        {
            //ClientToServerConfig clientToServerConfig = new ClientToServerConfig(10, true, client.ClientId);
            //string requestJson = ClientJsonDataSerializer.SerializeAction(clientToServerConfig);
            //await client.SendDataAsync(requestJson);
        }

        private async void InitializeClient()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            client = new Net.Client(endPoint);
            await client.ConnectAsync();
            //await client.ListenForPlayerInfoAsync();
            //await client.ListenForMessagesAsync();
        }

        private void Client_MessageReceived(object? sender, MessageBase message)
        {
            if(message is ToGameBaseMessage toGameMessage)
            {
                toGameMessage.Handle(this);
            }
        }
    }
}