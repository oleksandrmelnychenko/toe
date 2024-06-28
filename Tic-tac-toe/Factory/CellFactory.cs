using System;
using System.Collections.Generic;
using Tic_tac_toe.Models;

namespace Tic_tac_toe.Fabric
{
    public static class CellFactory
    {
        public static List<Cell> Build(int count, CellType cellType)
        {
            List<Cell> cells = new List<Cell>();
            switch (cellType)
            {
                case CellType.Cell:
                    for (int i = 0; i < count; i++)
                    {
                        cells.Add(new Cell());
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid cell type.");
            }
            return cells;
        }
    }
}
