﻿using Prism.Commands;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
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

        public bool IsActiveBoard { get; set; }
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
            ServerGameMessage serverMessage = ClientJsonDataSerializer.DeserializeServerMessage(message);
            gameStatus = serverMessage.Status;
            ActionHistory = serverMessage.GameHistory;
            for (int i = 0; i < _boardBoardCells.Count; i++)
            {
                _boardBoardCells[i] = serverMessage.BoardCells[i];
            }

            UpdatePlayerData(serverMessage);
        }

        public void UpdatePlayerData(ServerGameMessage serverMessage)
        {
            if (client.Player.Id == serverMessage.Player.Id)
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

        private async Task ApplyGameAction(BoardCell boardCell)
        {
            UpdateBoardCell(boardCell);
            ClientGameMessage playerMove = new ClientGameMessage(client.ClientId, boardCell, false);
            string moveJson = ClientJsonDataSerializer.SerializeAction(playerMove);
            await client.SendDataAsync(moveJson);
        }

        private async Task RestartRequest()
        {
            ClientGameMessage restartRequest = new ClientGameMessage(client.ClientId, null!, true);
            string requestJson = ClientJsonDataSerializer.SerializeAction(restartRequest);
            await client.SendDataAsync(requestJson);
        }

        private void Client_MessageReceived(object? sender, string message)
        {
            UpdateGameData(message);
        }

    }

}