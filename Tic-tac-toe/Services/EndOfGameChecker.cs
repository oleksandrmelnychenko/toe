using System.Collections.Generic;
using Tic_tac_toe.Models;
using Tic_tac_toe.WinnerCombination;

namespace Tic_tac_toe.Service
{
    public class EndOfGameChecker
    {
        private WinnerCombinationBase _winnerCombination { get; set; }

        public EndOfGameChecker(WinnerCombinationBase winnerCombination)
        {
            _winnerCombination = winnerCombination;
        }

        public bool CheckForWinner(List<Cell> cells)
        {
            foreach (var combination in _winnerCombination.Combination)
            {
                string firstSymbol = cells[combination[0]].SymbolName;
                if (!string.IsNullOrEmpty(firstSymbol) &&
                    firstSymbol == cells[combination[1]].SymbolName &&
                    firstSymbol == cells[combination[2]].SymbolName)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckForDraw(List<Cell> cells)
        {
            foreach (Cell box in cells)
            {
                if(string.IsNullOrEmpty(box.SymbolName))
                {
                    return false;
                }
            }
            return true;
        }
    }

}
