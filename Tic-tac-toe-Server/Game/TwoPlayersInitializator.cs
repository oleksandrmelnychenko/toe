using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic_tac_toe_Server.Game.Interfaces;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Models;

namespace Tic_tac_toe_Server.Game
{
    internal class TwoPlayersInitializator : IPlayerInitializator
    {
        public Player[] InitializePlayers()
        {
            return new Player[]
            {
                new Player(SymbolsConst.SymbolX, true, Guid.NewGuid()),
                new Player(SymbolsConst.SymbolO, false, Guid.NewGuid())
            };
        }
    }
}
