using Microsoft.CodeAnalysis;
using System.Diagnostics;
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
                ClientToServerConfig clientGameMessage = ServerJsonDataSerializer.DeserializeAction(jsonMessage);
                UpdateGameData(clientGameMessage);

                await SendNewGameData(BoardCells[clientGameMessage.CellIndex]);
            }
            else
            {
                _logger.LogWarning("UpdateGameData input message is null!");
            }
        }

        public async Task SetStartData()
        {
            Status status = Status.Start;
            Net.ServerToClientConfig serverConfig = new(status, _gameMaster.GetCurrentPlayer().Id, null, null, _gameMaster.GetHistory());

            string serverConfigJson = ServerJsonDataSerializer.SerializeServerMessage(serverConfig);

            _logger.LogWarning($"{_gameMaster.GetCurrentPlayer().Id} the main client in game!!!");

            if (!string.IsNullOrEmpty(serverConfigJson))
            {
                await Server.SendDataToClientsAsync(serverConfigJson);
            }
        }

        private void UpdateGameData(ClientToServerConfig clientGameMessage)
        {
            if (clientGameMessage.IsRestart == true)
            {
                _gameMaster.StartNewGameSession();
                BoardCells = _gameMaster.GetActiveGameSessionBoard();
            }
            else
            {
                BoardCell cell = new(clientGameMessage.CellIndex, _gameMaster.GetCurrentPlayer().PlayerSymbolName, true);

                BoardCells[cell.Index] = cell;

                _gameMaster.NewAction(cell);
            }
        }

        private ushort GetNewCellIndex(ClientToServerConfig config)
        {
            return config.CellIndex;
        }

        private async Task SendNewGameData(BoardCell newCell)
        {
            Net.ServerToClientConfig serverConfig = new Net.ServerToClientConfig(_gameMaster.GetStatus(), _gameMaster.GetCurrentPlayer().Id, newCell.Index, GetCellSymbol(newCell), _gameMaster.GetHistory());

            string serverConfigJson = ServerJsonDataSerializer.SerializeServerMessage(serverConfig);

            await Server.SendDataToClientsAsync(serverConfigJson);
        }

        private Symbol GetCellSymbol(BoardCell cell)
        {
            TicTacToeGame.Client.Game.Symbol symbol;

            if(cell.Value.HasValue)
            {
                symbol = cell.Value.Value;
            }
            else
            {
                return Symbol.Empty;
            }

            if(symbol == TicTacToeGame.Client.Game.Symbol.X)
            {
                return Symbol.X;
            }
            else if((symbol == TicTacToeGame.Client.Game.Symbol.O))
            {
                return Symbol.O;
            }
            else
            {
                return Symbol.Empty;
            }
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
