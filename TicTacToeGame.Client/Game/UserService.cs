using Avalonia.Media.Imaging;
using System;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Models;
using TicTacToeGame.Client.Symbols;

namespace TicTacToeGame.Client.Game
{
    public class UserService
    {
        private readonly User[] _users = new User[2];
        private int _currentUserIndex;

        public User CurrentUser => _users[_currentUserIndex];

        public UserService()
        {
            InitializeUsers();
        }

        private void InitializeUsers()
        {
            _users[0] = new User(new Bitmap(SymbolPath.XPath), SymbolsConst.SymbolX, true);
            _users[1] = new User(new Bitmap(SymbolPath.OPath), SymbolsConst.SymbolO, false);

            _currentUserIndex = 0;
        }

        public void ChangeCurrentUser()
        {
            _currentUserIndex = _currentUserIndex == 0 ? 1 : 0;
        }

        public void SwapUsersSymbols()
        {
            if (_users[0].UserSymbolName == SymbolsConst.SymbolX)
            {
                _users[0].UserSymbolName = SymbolsConst.SymbolO;
                _users[1].UserSymbolName = SymbolsConst.SymbolX;
            }
            else
            {
                _users[0].UserSymbolName = SymbolsConst.SymbolX;
                _users[1].UserSymbolName = SymbolsConst.SymbolO;
            }
        }
    }
}
