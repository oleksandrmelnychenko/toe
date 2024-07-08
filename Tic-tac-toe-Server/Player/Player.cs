using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Game;

namespace Tic_tac_toe_Server.Player
{
    public class Player : PlayerBase
    {
        public Player(Symbol symbolName, bool isActive)
        {
            PlayerSymbolName = symbolName;
            Id = Guid.NewGuid();
            IsActived = isActive;
        }
    }
}
