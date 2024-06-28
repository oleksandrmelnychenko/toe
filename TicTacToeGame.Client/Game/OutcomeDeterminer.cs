using System.Collections.Generic;
using System.Linq;

namespace TicTacToeGame.Client.Game;

public static class OutcomeDeterminer
{
    public static bool IsWinner(IReadOnlyCollection<BoardCell> boardCells)
    {
        return GameWinningConditions.Combinations.Any(combination =>
        {
            string? firstSymbol = boardCells.ElementAt(combination[0]).Value;
            return !string.IsNullOrEmpty(firstSymbol) &&
                   firstSymbol == boardCells.ElementAt(combination[1]).Value &&
                   firstSymbol == boardCells.ElementAt(combination[2]).Value;
        });
    }

    public static bool IsDraw(IReadOnlyCollection<BoardCell> boardCells) =>
        AreAllCellsFilled(boardCells) && !IsWinner(boardCells);

    private static bool AreAllCellsFilled(IReadOnlyCollection<BoardCell> boardCells) =>
        boardCells.All(cell => !string.IsNullOrEmpty(cell.Value));
}