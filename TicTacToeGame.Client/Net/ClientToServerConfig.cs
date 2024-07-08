using System;

namespace TicTacToeGame.Client.Net
{
    public record ClientToServerConfig(ushort CellIndex, bool IsRestart, Guid ClientId);
}
