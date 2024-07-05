using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Models;

namespace Tic_tac_toe_Server.Net
{
    public record ServerGameMessage
    {
        public Status Status { get; set; }
        public List<BoardCell> BoardCells { get; set; }

        public string GameHistory { get; set; }

        public Player? User { get; set; }

        public ServerGameMessage(Status gameStatus, List<BoardCell> boardCells, string gameHistory, Player user)
        {
            Status = gameStatus;
            BoardCells = boardCells;
            GameHistory = gameHistory;
            User = user;
        }
    }
}
