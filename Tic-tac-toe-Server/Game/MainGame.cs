using System.Net;
using Tic_tac_toe_Server.Constants;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net;
using Tic_tac_toe_Server.Player.Factory;
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
            Server.ClientDisconect += ClientDisconect;
        }

        public async Task Start()
        {
            _gameMaster.StartNewGameSession();
            BoardCells = _gameMaster.GetActiveGameSessionBoard();
            Server.SetPlayerManager(_gameMaster.GetPlayerService());
            await Server.StartServerAsync();
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
            ServerGameMessage serverGameMessage = new ServerGameMessage(status, BoardCells, _gameMaster.GetHistory(), _gameMaster.GetCurrentPlayer());

            string startDataJson = ServerJsonDataSerializer.SerializeServerMessage(serverGameMessage);

            if (!string.IsNullOrEmpty(startDataJson))
            {
                await Server.SendDataToClientsAsync(startDataJson);
            }
        }

        private void UpdateGameData(string jsonMessage)
        {
            ClientGameMessage clientGameMessage = ServerJsonDataSerializer.DeserializePlayer(jsonMessage);

            if (clientGameMessage.RestartRequest == true)
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
            ServerGameMessage serverGameMessage = new ServerGameMessage(_gameMaster.GetStatus(), BoardCells, _gameMaster.GetHistory(), _gameMaster.GetCurrentPlayer());

            string boardCellsJson = ServerJsonDataSerializer.SerializeServerMessage(serverGameMessage);

            await Server.SendDataToClientsAsync(boardCellsJson);
        }

        private void Client_MessageReceived(object? sender, string message)
        {
            HandleClientMessage(message).GetAwaiter().GetResult();
        }

        private void ClientDisconect(object? sender, EventArgs eventArgs)
        {
        }
    }
}
