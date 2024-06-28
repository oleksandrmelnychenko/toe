using Avalonia.Media.Imaging;
using Tic_tac_toe.Models;

namespace Tic_tac_toe.Service
{
    internal class UserService
    {
        public User User1 { get; set; } = null!;
        public User User2 { get; set; } = null!;

        public User CurrentUser { get; set; } = null!;

        public void InitializeUsers()
        {
            User1 = new User(new Bitmap(Symbols.SymbolPath.XPath), Constants.SymbolsConst.SymbolX, true);
            User2 = new User(new Bitmap(Symbols.SymbolPath.OPath), Constants.SymbolsConst.SymbolO, false);

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
