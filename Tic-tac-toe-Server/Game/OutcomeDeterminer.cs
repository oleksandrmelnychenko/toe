using TicTacToeGame.Client.Game;

namespace Tic_tac_toe_Server.Game
{
    public static class OutcomeDeterminer
    {
        public static bool IsWinner(IReadOnlyCollection<BoardCell> boardCells)
        {
            return GameWinningConditions.Combinations.Any(combination =>
            {
                TicTacToeGame.Client.Game.Symbol? firstSymbol = boardCells.ElementAt(combination[0]).Value;
                return firstSymbol.HasValue && firstSymbol != TicTacToeGame.Client.Game.Symbol.Empty &&
                       firstSymbol == boardCells.ElementAt(combination[1]).Value &&
                       firstSymbol == boardCells.ElementAt(combination[2]).Value;
            });
        }

        public static bool IsDraw(IReadOnlyCollection<BoardCell> boardCells) =>
            AreAllCellsFilled(boardCells) && !IsWinner(boardCells);

        private static bool AreAllCellsFilled(IReadOnlyCollection<BoardCell> boardCells) =>
            boardCells.All(cell => cell.Value.HasValue && cell.Value != TicTacToeGame.Client.Game.Symbol.Empty);
    }
}
