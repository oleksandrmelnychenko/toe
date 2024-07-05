using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Models;

namespace Tic_tac_toe_Server.Net
{
    public record ServerGameMessage(Status Status, List<BoardCell> BoardCells, string GameHistory, Player Player);
}
