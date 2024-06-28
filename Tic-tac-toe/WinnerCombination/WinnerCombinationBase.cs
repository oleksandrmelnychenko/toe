using System.Collections.Generic;

namespace Tic_tac_toe.WinnerCombination
{
    public abstract class WinnerCombinationBase
    {
        public abstract List<List<int>> Combination { get; set; }
    }
}
