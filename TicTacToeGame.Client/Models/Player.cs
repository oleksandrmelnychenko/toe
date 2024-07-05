using Avalonia.Media.Imaging;
using System;

namespace TicTacToeGame.Client.Models
{
    public class Player(string playerSymbolName, bool isActived, Guid id)
    {
        public Guid Id { get; set; } = id;
        public string PlayerSymbolName { get; set; } = playerSymbolName;

        public bool IsActived { get; set; } = isActived;
    }
}
