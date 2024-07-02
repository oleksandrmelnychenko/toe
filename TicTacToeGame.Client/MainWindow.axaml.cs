using Avalonia.Controls;
using System.Net;

namespace TicTacToeGame.Client
{
    internal partial class MainWindow : Window
    {
        private Net.Client client;

        public MainWindow()
        {
            InitializeComponent();
            InitializeClient();
            DataContext = new MainViewModel(client!);
        }

        private async void InitializeClient()
        {
            client = new Net.Client(IPAddress.Parse("127.0.0.1"), 8888);
            await client.ConnectAsync();
            await client.ListenForMessagesAsync();
        }

        private void Binding(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }
    }

}