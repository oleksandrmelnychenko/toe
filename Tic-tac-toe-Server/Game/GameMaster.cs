using System;
using System.Collections.Generic;
using Tic_tac_toe_Server.Player;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Models;

namespace Tic_tac_toe_Server.Game
{
    public class GameMaster
    {
        private const ushort CellsCount = 9;

        private GameSession? _activeGameSession;

        /// <summary>
        ///     The GameMaster class represents the game master that manages the Tic Tac Toe game.
        /// </summary>
        public GameMaster()
        {

        }

        /// <summary>
        ///     Starts a new Tic Tac Toe game session.
        /// </summary>
        public void StartNewGameSession()
        {
            _activeGameSession = new GameSession(CreateNewBoard());
        }

        public PlayerBase GetCurrentPlayer()
        {
            ArgumentNullException.ThrowIfNull(nameof(_activeGameSession));
            return _activeGameSession!.GetCurrentPlayer();
        }

        public void NewAction(BoardCell boardCell)
        {
            ArgumentNullException.ThrowIfNull(nameof(_activeGameSession));
            GameAction action = new(_activeGameSession!.GetCurrentPlayer(), boardCell.Index);

            _activeGameSession!.HandleAction(action);
        }

        public PlayerManager GetPlayerService()
        {
            ArgumentNullException.ThrowIfNull(nameof(_activeGameSession));
            return _activeGameSession!.GetPlayerService();
        }

        public List<BoardCell> GetActiveGameSessionBoard()
        {
            ArgumentNullException.ThrowIfNull(nameof(_activeGameSession));
            return _activeGameSession!.BoardCells;
        }

        public Status GetStatus()
        {
            ArgumentNullException.ThrowIfNull(nameof(_activeGameSession));
            return _activeGameSession!.Status;
        }

        public string GetHistory()
        {
            ArgumentNullException.ThrowIfNull(nameof(_activeGameSession));
            return _activeGameSession!.History.ActionHistory;
        }

        private static List<BoardCell> CreateNewBoard() => CellFactory.Build(CellsCount);
    }
}