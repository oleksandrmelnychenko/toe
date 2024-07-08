using Avalonia.Media.Imaging;
using System;
using TicTacToeGame.Client.Game;

namespace TicTacToeGame.Client.Models
{
    public class Player(Symbol playerSymbolName, bool isActived, Guid id)
    {
        public Guid Id { get; set; } = id;
        public Symbol PlayerSymbolName { get; set; } = playerSymbolName;

        public bool IsActived { get; set; } = isActived;
    }
}
