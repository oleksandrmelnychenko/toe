using System;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Net.Messages
{
    public class NewGameDataMessage : MessageBase
    {
        public Status Status { get; set; }

        public Guid CurrentPlayerId { get; set; }

        public Symbol CurrentPlayerSymbol { get; set; }

        public ushort CellIndex { get; set; }

        public Symbol CellSymbol { get; set; }

        public string ActionHistory { get; set; }
    }
}
