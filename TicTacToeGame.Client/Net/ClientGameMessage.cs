using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Net
{
    public record ClientGameMessage(Guid Guid, BoardCell Cell, bool RestartRequest);
}
