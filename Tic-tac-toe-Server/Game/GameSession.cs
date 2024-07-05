using System.Collections.Generic;
using TicTacToeGame.Client.Models;
using Tic_tac_toe_Server.Game;
using TicTacToeGame.Client.Game;

namespace Tic_tac_toe_Server.Game
{
    public class GameSession(List<BoardCell> boardCells)
    {
        //Тут змінив тип на ліст, тому що чомусь не оновлюється IReadOnlyCollection хоча ніби роблю посилання на один об'єкт і в гілці main це працює, можливо це через те що там ObservableCollection
        public List<BoardCell> BoardCells { get; private set; } = boardCells;

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
            else if (OutcomeDeterminer.IsDraw(BoardCells))
            {
                Status = Status.Draw;
            }
            else
            {
                Status = Status.PlayerTurn;
            }

        }
    }
}
