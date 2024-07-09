using System.Net;
using Tic_tac_toe_Server.Constants;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Net;


namespace Tic_tac_toe_Server.Game 
{
    internal class MainGame : IDisposable
    {
        private GameMaster _gameMaster { get; set; } = new();

        private ILogger _logger;

        private bool _disposed = false;

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
            await SetStartData(Status.Start);
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

                if(clientGameMessage.IsRestart == true)
                {
                    _gameMaster.StartNewGameSession();
                    BoardCells = _gameMaster.GetActiveGameSessionBoard();
                    await SetStartData(Status.Restart);
                }
                else
                {
                    UpdateGameData(clientGameMessage);
                    await SendNewGameData(BoardCells[clientGameMessage.CellIndex]);
                }
            }
            else
            {
                _logger.LogWarning("UpdateGameData input message is null!");
            }
        }

        public async Task SetStartData(Status status)
        {
            Net.ServerToClientConfig serverConfig = new(status, _gameMaster.GetCurrentPlayer().Id, null, _gameMaster.GetCurrentPlayer().PlayerSymbolName, _gameMaster.GetCurrentPlayer().PlayerSymbolName, _gameMaster.GetHistory());

            string serverConfigJson = ServerJsonDataSerializer.SerializeServerMessage(serverConfig);

            if (!string.IsNullOrEmpty(serverConfigJson))
            {
                await Server.SendDataToClientsAsync(serverConfigJson);
            }
        }

        private void UpdateGameData(ClientToServerConfig clientGameMessage)
        {
            BoardCell cell = new(clientGameMessage.CellIndex, (TicTacToeGame.Client.Game.Symbol?)_gameMaster.GetCurrentPlayer().PlayerSymbolName, true);

            BoardCells[cell.Index] = cell;

            _gameMaster.NewAction(cell);
        }

        private async Task SendNewGameData(BoardCell newCell)
        {
            Net.ServerToClientConfig serverConfig = new Net.ServerToClientConfig(_gameMaster.GetStatus(), _gameMaster.GetCurrentPlayer().Id, newCell.Index, GetCellSymbol(newCell), _gameMaster.GetCurrentPlayer().PlayerSymbolName, _gameMaster.GetHistory());

            string serverConfigJson = ServerJsonDataSerializer.SerializeServerMessage(serverConfig);

            await Server.SendDataToClientsAsync(serverConfigJson);
        }

        private Symbol GetCellSymbol(BoardCell cell)
        {
            TicTacToeGame.Client.Game.Symbol symbol;

            if (cell.Value.HasValue)
            {
                symbol = cell.Value.Value;
            }
            else
            {
                return Symbol.Empty;
            }

            if (symbol == TicTacToeGame.Client.Game.Symbol.X)
            {
                return Symbol.X;
            }
            else if ((symbol == TicTacToeGame.Client.Game.Symbol.O))
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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Server.MessageReceived -= Client_MessageReceived;
                    Server.ClientDisconect -= ClientDisconect;

                    if(Server != null)
                    {
                        Server?.Dispose();
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
