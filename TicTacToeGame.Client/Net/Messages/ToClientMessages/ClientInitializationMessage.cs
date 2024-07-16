using System;

namespace TicTacToeGame.Client.Net.Messages.ToClientMessages
{
    internal class ClientInitializationMessage : ToClientBaseMessage
    {
        public Guid PlayerId { get; set; }
    }
}
