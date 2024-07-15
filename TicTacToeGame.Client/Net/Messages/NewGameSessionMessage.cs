using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeGame.Client.Net.Messages
{
    internal class NewGameSessionMessage : MessageBase
    {
        public override void Handle()
        {
            Debug.WriteLine("Handle new game session message.");
        }
    }
}
