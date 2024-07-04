using Avalonia.Media.Imaging;
using System;

namespace TicTacToeGame.Client.Models
{
    public class User(string userSymbolName, bool isActived, Guid id)
    {
        public Guid Id { get; set; } = id;
        public string UserSymbolName { get; set; } = userSymbolName;

        public bool IsActived { get; set; } = isActived;
    }
}
