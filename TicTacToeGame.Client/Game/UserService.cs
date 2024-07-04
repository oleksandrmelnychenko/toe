using Avalonia.Media.Imaging;
using System;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Models;
using TicTacToeGame.Client.Symbols;

namespace TicTacToeGame.Client.Game
{
    public class UserService
    {
        public readonly User[] _users = new User[2];
        private int _currentUserIndex;

        public User CurrentUser => _users[_currentUserIndex];

        public UserService()
        {
            InitializeUsers();
        }

        private void InitializeUsers()
        {
            _users[0] = new User(SymbolsConst.SymbolX, true, Guid.NewGuid());
            _users[1] = new User(SymbolsConst.SymbolO, false, Guid.NewGuid());

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
