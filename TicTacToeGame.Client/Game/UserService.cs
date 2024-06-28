using Avalonia.Media.Imaging;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Models;
using TicTacToeGame.Client.Symbols;

namespace TicTacToeGame.Client.Game
{
    internal class UserService
    {
        public User User1 { get; set; } = null!;
        public User User2 { get; set; } = null!;

        public User CurrentUser { get; set; } = null!;

        public void InitializeUsers()
        {
            User1 = new User(new Bitmap(SymbolPath.XPath), SymbolsConst.SymbolX, true);
            User2 = new User(new Bitmap(SymbolPath.OPath), SymbolsConst.SymbolO, false);

            CurrentUser = User1;
        }

        public void ChangeCurrentUser()
        {
            if (CurrentUser == User1)
            {
                CurrentUser = User2;
            }
            else
            {
                CurrentUser = User1;
            }
        }
    }
}
