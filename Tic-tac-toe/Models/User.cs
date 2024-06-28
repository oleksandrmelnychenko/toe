using Avalonia.Media.Imaging;

namespace Tic_tac_toe.Models
{
    public class User(Bitmap userSymbol, string userSymbolName, bool isActived)
    {
        public Bitmap UserSymbol { get; set; } = userSymbol;
        public string UserSymbolName { get; set; } = userSymbolName;

        public bool IsActived { get; set; } = isActived;
    }
}
