using Avalonia.Controls;

namespace TicTacToeGame.Client
{
    internal partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}