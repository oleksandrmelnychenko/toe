using System.Collections.Generic;
using System.ComponentModel;
using TicTacToeGame.Client.Game;
using Tic_tac_toe_Server.Player;

namespace Tic_tac_toe_Server.Game
{
    public class ActionHistory
    {
        private readonly List<GameAction> moves;

        public string History { get; set; } = string.Empty;

        public ActionHistory()
        {
            moves = new List<GameAction>();
        }

        public void AddAction(GameAction gameAction)
        {
            moves.Add(gameAction);
            History = History + $"Player {gameAction.Player.PlayerSymbolName} chose box {gameAction.BoxPosition}.\n";
        }

        public List<GameAction> GetAction()
        {
            return moves;
        }

        public void ClearHistory()
        {
            moves.Clear();
            History = string.Empty;
        }

    }
}
