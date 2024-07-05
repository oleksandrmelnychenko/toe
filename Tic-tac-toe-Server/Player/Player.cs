using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_tac_toe_Server.Player
{
    internal class Player : PlayerBase
    {
        public string PlayerSymbolName { get; set; }

        public bool IsActived { get; set; }
    }
}
