using System.Collections.Generic;
using System.Linq;

namespace TicTacToeGame.Client.Game
{
    public static class CellFactory
    {
        public static List<BoardCell> Build(ushort count) =>
            Enumerable.Range(0, count)
                      .Select(index => new BoardCell((ushort)index))
                      .ToList();
    }
}
