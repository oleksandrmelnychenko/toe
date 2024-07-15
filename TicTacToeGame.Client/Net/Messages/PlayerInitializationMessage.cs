using System;
using System.Diagnostics;

namespace TicTacToeGame.Client.Net.Messages
{
    internal class PlayerInitializationMessage : MessageBase
    {
        public Guid PlayerId { get; set; }

        public override void Handle()
        {
            Debug.WriteLine("Handle player message.");
        }
    }
}
