using System;

namespace TicTacToeGame.Client.Net.Messages
{
    internal class ClientInitializationMessage : MessageBase
    {
        public Guid PlayerId { get; set; }
    }
}
