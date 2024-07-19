using System;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Net.Messages
{
    public class NewGameSessionMessage : MessageBase
    {
        public Status Status { get; set; }

        public Guid CurrentPlayerId { get; set; }

        public Symbol CurrentPlayerSymbol { get; set; }
    }
}
