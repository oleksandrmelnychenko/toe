using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tic_tac_toe_Server.Net.Strategies;
using TicTacToeGame.Client.Net.Configs;
using TicTacToeGame.Client.Net.Messages;

namespace TicTacToeGame.Client.Net
{
    public class DataTransferManager
    {
        private readonly Client _client;

        private readonly MainViewModel _mainViewModel;

        public DataTransferManager(Client client, MainViewModel mainViewModel)
        {
            this._client = client;
            _mainViewModel = mainViewModel;

            client.MessageReceived += TransferReceivedData;
            mainViewModel.SubmitData += TransferProcessedData;
        }

        public void TransferProcessedData(ConfigBase config)
        {
            JsonValidationResult jsonMessage = Serializer.Serialize(config);
            if (jsonMessage.IsValid)
            {
                Task.Run(() => _client.SendDataAsync(jsonMessage.JsonMessage));
            }
            else
            {
                Debug.WriteLine("Serialization error!");
            }
        }

        public void TransferReceivedData(string message)
        {
            try
            {
                MessageBase messageBase = Serializer.ParseMessage(message);
                _mainViewModel.SetStrategy(MessageStrategyFactory.GetStrategy(messageBase.Type));
                _mainViewModel.OnMessageRecived(messageBase);
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
