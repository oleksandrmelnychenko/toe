using System.Collections.Generic;
using System.ComponentModel;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Models
{
    public class GameHistory
    {
        private List<GameAction> actions;

        public string History { get; set; } = string.Empty;

        public GameHistory()
        {
            actions = new List<GameAction>();
        }

        public void AddAction(GameAction gameAction)
        {
            actions.Add(gameAction);
            History = History + $"Player {gameAction.User.UserSymbolName} chose box {gameAction.CellIndex}.\n";
        }

        public List<GameAction> GetAction()
        {
            return actions;
        }

        public void ClearHistory()
        {
            actions.Clear();
        }

    }
}
