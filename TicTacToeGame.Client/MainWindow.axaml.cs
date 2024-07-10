using Avalonia.Controls;
using System.Net;
using System.Threading.Tasks;

namespace TicTacToeGame.Client
{
    internal partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }


        private void Binding(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }
    }

}