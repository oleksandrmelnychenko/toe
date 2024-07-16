using Avalonia.Media.TextFormatting.Unicode;
using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net;
using Tic_tac_toe_Server.Net.Messages;
using Tmds.DBus.Protocol;

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

        public void NewAction(NewActionMessage message)
        {
            (bool, GameSession) session = FindSessionByPlayerId(message.ClientId);
            if(session.Item1)
            {
                session.Item2!.HandleAction(message);
            }
            else
            {
                _logger.LogError($"Cannot find a session with a player with the specified id!");
            }
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
            MessageBase messageBase = Serializer.ParseMessage(message);
            messageBase.Handle(this);
        }

        private (bool, GameSession) FindSessionByPlayerId(Guid playerId)
        {
            foreach (var room in _rooms)
            {
                if (room.GetPlayerManager().HasPlayer(playerId));
                {
                    return (true, room);
                }
            }

            return (false, null);
        }

        private bool CheckForAvalibleSession() => _rooms.Any(r => r.IsFull == false);

        //public PlayerManager GetPlayerService()
        //{
        //    ArgumentNullException.ThrowIfNull(nameof(_rooms));
        //    return _rooms!.GetPlayerService();
        //}
    }
}