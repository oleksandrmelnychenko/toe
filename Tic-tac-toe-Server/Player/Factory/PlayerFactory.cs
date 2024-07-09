using TicTacToeGame.Client.Constants;
using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Player.Factory
{
    internal class PlayerFactory
    {
        private PlayerFactory()
        {

        }

        public static List<PlayerBase> CreatePlayers(int count)
        {
            var players = new List<PlayerBase>();
            for (int i = 0; i < count; i++)
            {
                if(i % 2 == 0)
                {
                    players.Add(new Player(Symbol.X, true));
                }
                else
                {
                    players.Add(new Player(Symbol.O, false));
                }
            }
            return players;
        }
    }
}
