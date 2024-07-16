using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeGame.Client.Net.Messages.ToGameMessages
{
    public abstract class ToGameBaseMessage : Messages.ToGameBaseMessage
    {
        public abstract void Handle(MainViewModel mainViewModel);
    }
}
