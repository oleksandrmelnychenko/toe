using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_tac_toe_Server.Player.Factory
{
    internal class PlayerFactory : PlayerFactoryBase
    {
        protected PlayerFactory()
        {

        }

        public override List<PlayerBase> CreatePlayers(int count)
        {
            var players = new List<PlayerBase>();
            for (int i = 0; i < count; i++)
            {
                players.Add(new Player());
            }
            return players;
        }
    }
}
