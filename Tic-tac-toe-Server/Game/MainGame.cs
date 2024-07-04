using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tic_tac_toe_Server.Constants;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Net;

namespace Tic_tac_toe_Server.Game
{
    internal class MainGame
    {
        public Server Server { get; set; }

        public List<BoardCell> BoardCells { get; set; }

        private GameMaster _gameMaster { get; set; } = new();

        private ILogger _logger;


        public MainGame(ILogger logger)
        {
            _logger = logger;
            Server = new Server(IPAddress.Parse(AddressConstants.IPAddress), AddressConstants.Port, logger);
            Server.MessageReceived += Client_MessageReceived;
        }

        public async Task StartMainGame()
        {
            _gameMaster.StartGame();
            BoardCells = new List<BoardCell>(_gameMaster.GetActiveGameSessionBoard());
            await Server.StartServerAsync(_gameMaster.GetUserService());
            await SetStartData();
            while (Server.IsActive == true)
            {
                Server.ListenClientsAsync().GetAwaiter().GetResult();
            }
        }


        private void Client_MessageReceived(object? sender, string message)
        {
            UpdateGameDataAsync(message).GetAwaiter().GetResult();
        }


        public async Task UpdateGameDataAsync(string jsonMessage)
        {
            if (!string.IsNullOrEmpty(jsonMessage))
            {
                ClientGameMessage clientGameMessage = ServerJsonDataSerializer.DeserializeMove(jsonMessage);

                BoardCell cell = clientGameMessage.Cell;

                Guid clientId = clientGameMessage.Guid;

                BoardCells[cell.Index] = cell;

                _gameMaster.NewAction(cell);

                ServerGameMessage serverGameMessage = new ServerGameMessage(_gameMaster.GetStatus(), BoardCells, _gameMaster.GetHistory(), _gameMaster.GetCurrentUser());

                string boardCellsJson = ServerJsonDataSerializer.SerializeServerMessage(serverGameMessage);

                await Server.SendDataToClientsAsync(boardCellsJson);
            }
            else
            {
                _logger.LogWarning("UpdateGameData input message is null!");
            }
        }

        public async Task SetStartData()
        {
            Status status = Status.Start;
            ServerGameMessage serverGameMessage = new ServerGameMessage(status, BoardCells, _gameMaster.GetHistory(), _gameMaster.GetCurrentUser());

            string startDataJson = ServerJsonDataSerializer.SerializeServerMessage(serverGameMessage);

            if(!string.IsNullOrEmpty(startDataJson))
            {
                await Server.SendDataToClientsAsync(startDataJson);
            }
        }

    }
}
