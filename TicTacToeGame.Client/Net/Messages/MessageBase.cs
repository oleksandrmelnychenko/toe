using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeGame.Client.Net.Messages
{
    public abstract class MessageBase
    {
        public MessageType Type { get; set; }
    }
}
