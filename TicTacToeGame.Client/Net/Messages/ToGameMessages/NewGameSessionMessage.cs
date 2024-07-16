using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Net.Messages.ToGameMessages
{
    public class NewGameSessionMessage : ToGameBaseMessage
    {
        public Status Status { get; set; }

        public Guid CurrentPlayerId { get; set; }

        public Symbol CurrentPlayerSymbol { get; set; }

        public override void Handle(MainViewModel mainViewModel)
        {
            mainViewModel.NewGameSession(this);
        }
    }
}
