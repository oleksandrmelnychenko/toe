using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Net.Messages.ToGameMessages
{
    public class NewGameDataMessage : ToGameBaseMessage
    {
        public Status Status { get; set; }

        public Guid CurrentPlayerId { get; set; }

        public Symbol CurrentPlayerSymbol { get; set; }

        public ushort CellIndex { get; set; }

        public Symbol CellSymbol { get; set; }

        public string ActionHistory { get; set; }

        public override void Handle(MainViewModel mainViewModel)
        {
            mainViewModel.UpdateGameData(this);
        }
    }
}
