using System;
using System.Collections.Generic;
using TicTacToeGame.Client.Models;

namespace TicTacToeGame.Client.Game;

public class GameMaster
{
    private const ushort CellsCount = 9;

    private GameSession? _activeGameSession;

    private PlayerManager _userService = new();

    /// <summary>
    ///     The GameMaster class represents the game master that manages the Tic Tac Toe game.
    /// </summary>
    public GameMaster()
    {
        
    }

    /// <summary>
    ///     Starts a new Tic Tac Toe game session.
    /// </summary>
    public void StartGame()
    {
        _activeGameSession = new GameSession(CreateNewBoard());
    }

    public void NewAction(BoardCell boardCell)
    {
        ArgumentNullException.ThrowIfNull(nameof(_activeGameSession));
        GameAction action = new(_userService.CurrentUser, boardCell.Index);

        _activeGameSession!.HandleAction(action);

        HandleGameStatus();
    }

    public Player GetCurrentUser()
    {
        return _userService.CurrentUser;
    }

    public IReadOnlyCollection<BoardCell> GetActiveGameSessionBoard()
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
        return _activeGameSession!.History.History;
    }

    private void HandleGameStatus()
    {
        if (_activeGameSession!.Status == Status.PlayerTurn)
        {
            _userService.ChangeCurrentUser();
        }
    }

    private static List<BoardCell> CreateNewBoard() => CellFactory.Build(CellsCount);
}