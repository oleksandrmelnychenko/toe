using System.Collections.Generic;
using System.ComponentModel;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Models
{
    public class GameHistory : INotifyPropertyChanged
    {
        private List<GameAction> moves;

        public string HistoryField { get; set; } = string.Empty;

        public GameHistory()
        {
            moves = new List<GameAction>();
        }

        public void AddMove(GameAction gameAction)
        {
            moves.Add(gameAction);
            HistoryField = HistoryField + $"Player {gameAction.User.UserSymbolName} chose box {gameAction.BoxPosition}.\n";
            OnPropertyChanged(nameof(HistoryField));
        }

        public List<GameAction> GetMoves()
        {
            return moves;
        }

        public void ClearHistory()
        {
            moves.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
