using System.Collections.ObjectModel;
using System.Net;
using Tic_tac_toe_Server.Constants;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Net;

namespace Tic_tac_toe_Server.Game
{
    internal class MainGame
    {
        private GameMaster _gameMaster { get; set; } = new();

        private ILogger _logger;

        private List<BoardCell> _boardCells = null!;

        public Server Server { get; set; }

        public List<BoardCell> BoardCells
        {
            get => _boardCells;
            set => _boardCells = value;
        }

        public MainGame(ILogger logger)
        {
            _logger = logger;
            Server = new Server(IPAddress.Parse(AddressConstants.IPAddress), AddressConstants.Port, logger);
            Server.MessageReceived += Client_MessageReceived;
        }

        public async Task StartMainGame()
        {
            _gameMaster.StartNewGameSession();
            BoardCells = _gameMaster.GetActiveGameSessionBoard();
            await Server.StartServerAsync(_gameMaster.GetUserService());
            await SetStartData();
            while (Server.IsActive == true)
            {
                await Server.ListenClientsAsync();
            }
        }

        public async Task HandleClientMessage(string jsonMessage)
        {
            if (!string.IsNullOrEmpty(jsonMessage))
            {
                UpdateGameData(jsonMessage);

                await SendNewGameData();
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

            if (!string.IsNullOrEmpty(startDataJson))
            {
                await Server.SendDataToClientsAsync(startDataJson);
            }
        }

        private void UpdateGameData(string jsonMessage)
        {
            ClientGameMessage clientGameMessage = ServerJsonDataSerializer.DeserializePlayer(jsonMessage);

            if(clientGameMessage.RestartRequest == true)
            {
                _gameMaster.StartNewGameSession();
                BoardCells = _gameMaster.GetActiveGameSessionBoard();
            }
            else
            {
                BoardCell cell = clientGameMessage.Cell;

                Guid clientId = clientGameMessage.Guid;

                BoardCells[cell.Index] = cell;

                _gameMaster.NewAction(cell);
            }
        }

        private async Task SendNewGameData()
        {
            ServerGameMessage serverGameMessage = new ServerGameMessage(_gameMaster.GetStatus(), BoardCells, _gameMaster.GetHistory(), _gameMaster.GetCurrentUser());

            string boardCellsJson = ServerJsonDataSerializer.SerializeServerMessage(serverGameMessage);

            await Server.SendDataToClientsAsync(boardCellsJson);
        }

        private void Client_MessageReceived(object? sender, string message)
        {
            HandleClientMessage(message).GetAwaiter().GetResult();
        }
    }
}
