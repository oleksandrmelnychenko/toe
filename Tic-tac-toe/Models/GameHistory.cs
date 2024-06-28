using System.Collections.Generic;
using System.ComponentModel;

namespace Tic_tac_toe.Models
{
    public class GameHistory : INotifyPropertyChanged
    {
        private List<Move> moves;

        public string HistoryField { get; set; } = string.Empty;

        public GameHistory()
        {
            moves = new List<Move>();
        }

        public void AddMove(Move move)
        {
            moves.Add(move);
            HistoryField = HistoryField + $"Player {move.User.UserSymbolName} chose box {move.BoxPosition}.\n";
            OnPropertyChanged(nameof(HistoryField));
        }

        public List<Move> GetMoves()
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
