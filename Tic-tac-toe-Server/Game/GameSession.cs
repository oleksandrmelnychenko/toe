using System.Collections.Generic;
using TicTacToeGame.Client.Models;

namespace TicTacToeGame.Client.Game;

public class GameSession(IReadOnlyCollection<BoardCell> boardCells)
{
    public IReadOnlyCollection<BoardCell> BoardCells { get; private set; } = boardCells;

    public Status Status { get; set; } = Status.Start;

    public GameHistory History { get; set; } = new();

    public void HandleAction(GameAction action)
    {
        History.AddAction(action);
        UpdateGameStatus();
    }

    public void UpdateGameStatus()
    {
        if (OutcomeDeterminer.IsWinner(BoardCells))
        {
            Status = Status.Finish;
        }
        else if(OutcomeDeterminer.IsDraw(BoardCells))
        {
            Status = Status.Draw;
        }
        else 
        { 
            Status = Status.PlayerTurn;
        }

    }
}
