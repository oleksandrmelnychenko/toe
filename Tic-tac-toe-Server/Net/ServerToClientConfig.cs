using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Player;
using TicTacToeGame.Client.Game;

namespace Tic_tac_toe_Server.Net
{
    public record ServerToClientConfig(Status Status, Guid CurrentPlayerId, ushort? CellIndex, Game.Symbol? Symbol, string GameHistory);
}
