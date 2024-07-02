using Avalonia.Controls;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using TicTacToeGame.Client.Net;

namespace TicTacToeGame.Client
{
    internal partial class MainWindow : Window
    {
        public MainWindow()
        {
            Net.Client client = new Net.Client(IPAddress.Parse("127.0.0.1"), 8888);
            client.ConnectAsync().GetAwaiter().GetResult();
            InitializeComponent();
            DataContext = new MainViewModel(client);
        }
        private void Binding(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }
    }
}