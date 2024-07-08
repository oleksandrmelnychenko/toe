using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Net
{
    public record ServerToClientConfig(Status Status, Guid CurrentPlayerId, ushort? CellIndex, Symbol? Symbol, string GameHistory);
}
