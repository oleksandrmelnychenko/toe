using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Game
{
    public static class OutcomeDeterminer
    {
        public static bool IsWinner(IReadOnlyCollection<BoardCell> boardCells)
        {
            return GameWinningConditions.Combinations.Any(combination =>
            {
                Symbol? firstSymbol = boardCells.ElementAt(combination[0]).Value;
                return firstSymbol.HasValue && firstSymbol != Symbol.Empty &&
                       firstSymbol == boardCells.ElementAt(combination[1]).Value &&
                       firstSymbol == boardCells.ElementAt(combination[2]).Value;
            });
        }

        public static bool IsDraw(IReadOnlyCollection<BoardCell> boardCells) =>
            AreAllCellsFilled(boardCells) && !IsWinner(boardCells);

        private static bool AreAllCellsFilled(IReadOnlyCollection<BoardCell> boardCells) =>
            boardCells.All(cell => cell.Value != Symbol.Empty);

    }
}
