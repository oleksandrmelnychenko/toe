using System.Collections.Generic;
using System.ComponentModel;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Models
{
    public class GameHistory
    {
        private List<GameAction> moves;

        public string ActionHistory { get; set; } = string.Empty;

        public GameHistory()
        {
            moves = new List<GameAction>();
        }

        public void AddAction(GameAction gameAction)
        {
            moves.Add(gameAction);
            ActionHistory = ActionHistory + $"Player {gameAction.User.UserSymbolName} chose box {gameAction.BoxPosition}.\n";
        }

        public List<GameAction> GetAction()
        {
            return moves;
        }

        public void ClearHistory()
        {
            moves.Clear();
        }

    }
}
