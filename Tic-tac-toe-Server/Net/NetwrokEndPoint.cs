using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net.Configs;
using Tic_tac_toe_Server.Net.Messages;

namespace Tic_tac_toe_Server.Net
{
    public class NetwrokEndPoint : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();

        private readonly ILogger _logger;

        private bool _disposed = false;

        private Socket _networkEndPoint;

        private ConcurrentDictionary<Guid, RemotePeer> _remotePeers = new();

        public event Action<string> OnReceived = delegate { };

        public NetwrokEndPoint(IPEndPoint iPEndPoint, ILogger logger)
        {
            _logger = logger;
            _networkEndPoint = Initialize(iPEndPoint);
        }

        public void StartServer()
        {
            try
            {
                _networkEndPoint.Listen();
                _logger.LogSuccess("Server start successful.");
                Task.Run(() => AcceptClients(cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                HandleServerException(ex, "start");
            }
        }

        public void StopServer()
        {
            try
            {
                cancellationTokenSource.Cancel();
                _networkEndPoint.Shutdown(SocketShutdown.Both);
                _networkEndPoint.Close();
                _logger.LogSuccess("Server stop successful.");
            }
            catch (Exception ex)
            {
                HandleServerException(ex, "stop");
            }
        }

        public async Task SendDataToRemotePeers(List<Guid> clientsIds, string message)
        {
            List<RemotePeer> remotePeers = _remotePeers.Values.Where(c => clientsIds.Contains(c.Id)).ToList();

            foreach (RemotePeer remotePeer in remotePeers)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message + "\n");

                if (remotePeer.RemoteEndPoint.Connected)
                {
                    try
                    {
                        NetworkStream networkStream = new NetworkStream(remotePeer.RemoteEndPoint);

                        await networkStream.WriteAsync(bytes, 0, bytes.Length);

                        _logger.LogMessage($"Message send to remotePeers: {remotePeer.Id}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }
                else
                {
                    _logger.LogWarning($"Client {remotePeer.Id} is not connected.");
                }
            }
        }

        public async Task AcceptClients(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Socket socket = await _networkEndPoint.AcceptAsync().ConfigureAwait(false);

                    RemotePeer remotePeer = new(socket, _logger, false, new NetworkStream(socket), Client_DataReceived, Client_Disconnected);

                    _remotePeers.TryAdd(remotePeer.Id, remotePeer);

                    _logger.LogMessage($"Client {remotePeer.Id} connected.");

                    await SendInitializeClientData(remotePeer).ConfigureAwait(false);
                    _ = Task.Run(() => remotePeer.StartReceiveAsync(), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogMessage("Client acceptance was canceled.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception while accepting remotePeer: {ex.Message}");
                }
            }
        }

        private void Client_DataReceived(string message)
        {
            try
            {
                OnReceived.Invoke(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in OnReceived handler: {ex.Message}");
            }
        }

        private void Client_Disconnected(Guid id)
        {
            ClientDisconnectedConfig config = new ClientDisconnectedConfig(id);
            JsonValidationResult validationResult = Serializer.Serialize(config);

            if(validationResult.IsValid)
            {
                OnReceived.Invoke(validationResult.JsonMessage);
                DisposeClient(id);
            }
            else
            {
                _logger.LogError($"Client disconected error: {validationResult.JsonMessage}");
            }
        }

        private async Task SendInitializeClientData(RemotePeer remotePeer)
        {
            PlayerInitializationConfig config = new PlayerInitializationConfig(remotePeer.Id);
            JsonValidationResult jsonValidationResult = Serializer.Serialize(config);

            if (jsonValidationResult.IsValid)
            {
                await SendDataToRemotePeer(remotePeer, jsonValidationResult.JsonMessage);
                _logger.LogMessage($"Initialize message send to remotePeer {remotePeer.Id}");
            }
            else
            {
                _logger.LogError($"Serialization problem for remotePeer {remotePeer.Id}: {jsonValidationResult.JsonMessage}");

                _remotePeers.TryRemove(remotePeer.Id, out _);
                remotePeer.RemoteEndPoint.Dispose();
                _logger.LogMessage($"Client {remotePeer.Id} disconnected due to serialization error.");
            }
        }

        private Socket Initialize(IPEndPoint endPointAddress)
        {
            Socket networkEndPoint = new Socket(SocketType.Stream, ProtocolType.Tcp);

            try
            {
                networkEndPoint.Bind(endPointAddress);
                return networkEndPoint;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private async Task SendDataToRemotePeer(RemotePeer remotePeer, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message + "\n");

            if (remotePeer.RemoteEndPoint.Connected)
            {
                try
                {
                    await remotePeer.Stream.WriteAsync(bytes, 0, bytes.Length);

                    _logger.LogMessage($"Message send to remotePeer: {remotePeer.Id}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            else
            {
                _logger.LogWarning($"Client {remotePeer.Id} is not connected.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DisposeClients();
                    DisposeNetworkEndPoint();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void DisposeClients()
        {
            foreach (var client in _remotePeers.Values)
            { 
                client.Dispose();
            }
        }

        private void DisposeClient(Guid id)
        {
            RemotePeer peer = _remotePeers.Values.First(p => p.Id == id);
            _remotePeers.TryRemove(peer.Id, out _);
            peer.Dispose();
        }

        private void DisposeNetworkEndPoint()
        {
            if (_networkEndPoint.Connected)
            {
                StopServer();
            }
            _networkEndPoint.Dispose();
        }

        private void HandleServerException(Exception ex, string action)
        {
            _logger.LogError($"Exception during server {action}: {ex.Message}");
            _networkEndPoint.Dispose();
        }

    }
}
