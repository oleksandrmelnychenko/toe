using System;
using System.Collections.Generic;
using TicTacToeGame.Client.Models;

namespace TicTacToeGame.Client.Game;

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
    public void StartGame()
    {
        _activeGameSession = new GameSession(CreateNewBoard());
    }

    public void NewAction(BoardCell boardCell)
    {
        // GameAction action = new();
        
       // _activeGameSession.HandleAction();
    }

    public IReadOnlyCollection<BoardCell> GetActiveGameSessionBoard()
    {
        ArgumentNullException.ThrowIfNull(nameof(_activeGameSession));
        return _activeGameSession!.BoardCells;
    }
    
    private static List<BoardCell> CreateNewBoard() => CellFactory.Build(CellsCount);
}