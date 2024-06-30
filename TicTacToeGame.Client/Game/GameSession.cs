using System.Collections.Generic;

namespace TicTacToeGame.Client.Game;

public class GameSession(IReadOnlyCollection<BoardCell> boardCells)
{
    public IReadOnlyCollection<BoardCell> BoardCells { get; set; } = boardCells;
    
    public Status Status { get; set; } = Status.Start;
    
 
    public void HandleAction(GameAction action)
    {
        
        
    }
}
