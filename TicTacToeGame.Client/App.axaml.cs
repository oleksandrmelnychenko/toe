using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Net;
using System.Threading.Tasks;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Net;

namespace TicTacToeGame.Client
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private readonly MainViewModel _mainViewModel = new MainViewModel();
        private Net.Client _client = new Net.Client();
        private DataTransferManager _dataTransferManager;

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = _mainViewModel
                };

                desktop.ShutdownRequested += DesktopOnShutdownRequested;
            }

            base.OnFrameworkInitializationCompleted();
            IPEndPoint iPEndPoint = new(IPAddress.Parse(NetworkAddressConfig.IPAddress), NetworkAddressConfig.Port);
            _dataTransferManager = new(_client, _mainViewModel);
            InitializeClient(iPEndPoint);
        }


        private bool _canClose;
        private void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
        {
            e.Cancel = !_canClose;

            if (!_canClose)
            {
                _client.Dispose();

                _canClose = true;
                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown();
                }
            }
        }

        private void InitializeClient(IPEndPoint iPEndPoint)
        {
            Task.Run(() => _client.ConnectAsync(iPEndPoint));
        }
    }
}