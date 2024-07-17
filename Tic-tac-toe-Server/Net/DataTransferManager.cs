using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net.Messages;

namespace Tic_tac_toe_Server.Net
{
    //Доробити
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
                Task.Run(() => _server.SendDataToClients(ids, jsonMessage.JsonMessage));
            }
            else
            {
                _logger.LogError("Serialization error!");
            }
        }

        public void TransferReceivedData(string message)
        {
            _master.OnMessageRecived(message);
        }
    }
}
