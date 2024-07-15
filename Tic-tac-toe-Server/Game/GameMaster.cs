using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Messages;
using Tic_tac_toe_Server.Net;
using TicTacToeGame.Client.Game;

namespace Tic_tac_toe_Server.Game
{
    public class GameMaster
    {
        private List<GameSession> _rooms = new List<GameSession>();

        private ILogger _logger;

        private Server _server;

        /// <summary>
        ///     The GameMaster class represents the game master that manages the Tic Tac Toe game.
        /// </summary>
        public GameMaster(Server server, ILogger logger)
        {
            _server = server;
            _logger = logger;
            _server.ClientConnected += OnClientConnected;
            _server.MessageReceived += OnMessageRecived;
            _server.StartServer();
        }

        /// <summary>
        ///     Starts a new Tic Tac Toe game session, and return it.
        /// </summary>
        public GameSession StartNewGameSession()
        {
            GameSession gameSession = new GameSession(_logger);
            _rooms.Add(gameSession);

            return gameSession;
        }

        public void NewAction(BoardCell boardCell, Guid gameSessionId)
        {
            ArgumentNullException.ThrowIfNull(nameof(_rooms));
            GameSession room = _rooms!.FirstOrDefault(r => r.Id == gameSessionId);
            ArgumentNullException.ThrowIfNull(nameof(room));
            GameAction action = new(room!.GetCurrentPlayer(), boardCell.Index);

            room!.HandleAction(action);
        }

        private void OnClientConnected(Guid clientId)
        {
            if (CheckForAvalibleSession())
            {
                GameSession gameSession = _rooms.FirstOrDefault(r => r.IsFull == false)!;

                gameSession.AddPlayer(clientId);

                SendInitialSessionData(gameSession);
            }
            else
            {
                GameSession gameSession = StartNewGameSession();

                gameSession.AddPlayer(clientId);
            }
        }

        private void SendInitialSessionData(GameSession gameSession)
        {
            if (gameSession.IsFull)
            {
                List<Guid> playersIds = gameSession.GetSessionPlayers();
                NewSessionConfig config = gameSession.GetStartSessionData();
                JsonValidationResult json = Serializer.SerializeNewSession(config);

                if (json.IsValid)
                {
                    Task.Run(() => _server.SendDataToClients(playersIds, json.JsonMessage));
                }
                else
                {
                    _logger.LogError($"Invalid json format!");
                }
            }
        }

        private void OnMessageRecived(string message)
        {
            _logger.LogMessage(message);
        }

        private bool CheckForAvalibleSession() => _rooms.Any(r => r.IsFull == false);

        //public PlayerManager GetPlayerService()
        //{
        //    ArgumentNullException.ThrowIfNull(nameof(_rooms));
        //    return _rooms!.GetPlayerService();
        //}
    }
}