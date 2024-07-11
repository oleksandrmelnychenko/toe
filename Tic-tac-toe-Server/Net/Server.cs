using System.Net;
using System.Net.Sockets;
using Tic_tac_toe_Server.Logging;

namespace Tic_tac_toe_Server.Net
{
    internal class Server
    {
        private Socket _server;

        private IPEndPoint _ipEndPoint;

        private List<Client> _clients = new List<Client>();

        private readonly object _clientLock = new();

        private readonly ILogger _logger;

        public Server(IPEndPoint iPEndPoint, ILogger logger)
        {
            _logger = logger;
            Initialize(ref iPEndPoint, ref _server);
        }

        public void StartServer()
        {
            try
            {
                _server.Listen();
                _logger.LogSuccess("Server start successful.");
                AcceptClientsAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                _server.Dispose();
            }
        }

        public void StopServer()
        {
            try
            {
                _server.Shutdown(SocketShutdown.Both);
                _server.Close();
                _logger.LogSuccess("Server stop successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                _server.Dispose();
            }
        }

        public async Task AcceptClientsAsync()
        {
            while (true)
            {
                Client client = new();
                client.Socket = await _server.AcceptAsync();
                lock (_clientLock)
                {
                    _clients.Add(client);
                }
            }
        }

        private void Initialize(ref IPEndPoint iPEndPoint, ref Socket server)
        {
            server = new Socket(SocketType.Stream, ProtocolType.Tcp);

            while (!_server.IsBound)
            {
                (bool isValid, string endPointValidationMessage) = EndPointValidation.IsValidEndPoint(iPEndPoint);

                if (isValid)
                {
                    try
                    {
                        _server.Bind(iPEndPoint);
                        _ipEndPoint = iPEndPoint;
                        _logger.LogSuccess("Server binding successful.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        UpdateEndPoint(iPEndPoint);
                        continue;
                    }
                }
                else
                {
                    _logger.LogError(endPointValidationMessage);
                    UpdateEndPoint(iPEndPoint);
                    continue;
                }
            }
        }

        private void UpdateEndPoint(IPEndPoint iPEndPoint)
        {
            _logger.LogMessage("Please enter new IP address:");
            IPAddress ipAddress;
            if (IPAddress.TryParse(Console.ReadLine(), out ipAddress))
            {
                iPEndPoint.Address = ipAddress;

                _logger.LogMessage("Please enter new port:");
                int port;

                if (int.TryParse(Console.ReadLine(), out port))
                {
                    iPEndPoint.Port = port;
                }
            }
        }
    }
}
