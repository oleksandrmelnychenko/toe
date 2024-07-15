using System.Collections.Generic;
using System.Linq;

namespace Tic_tac_toe_Server.Game
{
    public static class CellFactory
    {
        public static List<BoardCell> Build(ushort count) =>
            Enumerable.Range(0, count)
                      .Select(index => new BoardCell((ushort)index))
                      .ToList();
    }
}
