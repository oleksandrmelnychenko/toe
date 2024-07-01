using Avalonia.Controls;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TicTacToeGame.Client
{
    internal partial class MainWindow : Window
    {
        public MainWindow()
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse("127.0.0.1"), 8888);
            if (tcpClient.Connected)
            {
                Debug.WriteLine("User 1 connected to server");
            }
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        public async Task SendDataToServerAsync()
        {
            TcpClient client = new();
            try
            {
                await client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 8888);
                if(client.Connected)
                {
                    Debug.WriteLine("User 1 connected to server");
                }
                //NetworkStream stream = client.GetStream();

                //var message = $" {DateTime.Now} ";
                //var dateTimeBytes = Encoding.UTF8.GetBytes(message);
                //await stream.WriteAsync(dateTimeBytes, 0, dateTimeBytes.Length);
                //Console.WriteLine("Data sent to server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void Binding(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }
    }
}