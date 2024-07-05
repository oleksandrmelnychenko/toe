using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_tac_toe_Server.Player.Factory
{
    internal abstract class PlayerFactoryBase
    {
        public abstract List<PlayerBase> CreatePlayers(int count);
    }

}
