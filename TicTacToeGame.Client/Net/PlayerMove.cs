using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Net
{
    public class PlayerMove(Guid guid, BoardCell cell)
    {
        public Guid Id { get; set; } = guid;

        public BoardCell Cell { get; set; } = cell;
    }
}
