using Avalonia.Controls;
using System;
using Tic_tac_toe.Models;
using Tic_tac_toe.Service;
using Tic_tac_toe.ViewModel;
using Tic_tac_toe.WinnerCombination;

namespace Tic_tac_toe
{
    internal partial class MainWindow : Window
    {
        private UserService _userService;
        private GameHistory _gameHistory;
        public MainWindow()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            InitializeComponent();
            _userService = new UserService();
            _userService.InitializeUsers();
            _gameHistory = new GameHistory();
            StandartWinnerCombination _winnerCombination = new StandartWinnerCombination();
            DataContext = new MainWindowViewModel(_userService, _winnerCombination, _gameHistory);
        }
    }
}
