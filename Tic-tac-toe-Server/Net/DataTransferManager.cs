using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net.Messages;
using Tic_tac_toe_Server.Net.Strategies;

namespace Tic_tac_toe_Server.Net
{
    public class DataTransferManager
    {
        private readonly Server _server;

        private readonly GameMaster _master;

        private readonly ILogger _logger;

        public DataTransferManager(Server server, GameMaster gameMaster, ILogger logger)
        {
            this._server = server;
            _master = gameMaster;
            _logger = logger;

            server.MessageReceived += TransferReceivedData;
            gameMaster.SubmitData += TransferProcessedData;
        }

        public void TransferProcessedData(ConfigBase config, List<Guid> ids)
        {
            JsonValidationResult jsonMessage = Serializer.Serialize(config);
            if (jsonMessage.IsValid)
            {
                Task.Run(() => _server.SendDataToRemotePeers(ids, jsonMessage.JsonMessage));
            }
            else
            {
                _logger.LogError("Serialization error!");
            }
        }

        public void TransferReceivedData(string message)
        {
            try
            {
                MessageBase messageBase = Serializer.ParseMessage(message);
                _master.SetStrategy(MessageStrategyFactory.GetStrategy(messageBase.Type));
                _master.OnMessageRecived(messageBase);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
