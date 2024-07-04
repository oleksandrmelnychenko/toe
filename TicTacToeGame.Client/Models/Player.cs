using Avalonia.Media.Imaging;

namespace TicTacToeGame.Client.Models
{
    public class Player(Bitmap userSymbol, string userSymbolName, bool isActived)
    {
        public Bitmap UserSymbol { get; set; } = userSymbol;
        public string UserSymbolName { get; set; } = userSymbolName;

        public bool IsActived { get; set; } = isActived;
    }
}
