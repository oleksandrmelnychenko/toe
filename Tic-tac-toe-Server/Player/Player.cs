using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_tac_toe_Server.Player
{
    public class Player : PlayerBase
    {
        public Player(string symbolName, bool isActive)
        {
            PlayerSymbolName = symbolName;
            Id = Guid.NewGuid();
            IsActived = isActive;
        }
    }
}
