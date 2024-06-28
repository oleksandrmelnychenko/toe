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

        public bool CheckForWinner(Cell[] boxes)
        {
            foreach (var combination in _winnerCombination.Combination)
            {
                string firstSymbol = boxes[combination[0]].SymbolName;
                if (!string.IsNullOrEmpty(firstSymbol) &&
                    firstSymbol == boxes[combination[1]].SymbolName &&
                    firstSymbol == boxes[combination[2]].SymbolName)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckForDraw(Cell[] boxes)
        {
            foreach (Cell box in boxes)
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
