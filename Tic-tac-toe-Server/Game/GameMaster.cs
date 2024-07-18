using Tic_tac_toe_Server.Logging;
using Tic_tac_toe_Server.Net;
using Tic_tac_toe_Server.Net.Messages;

namespace Tic_tac_toe_Server.Game
{
    public class GameMaster
    {
        private List<GameSession> _rooms = new List<GameSession>();

        private ILogger _logger;

        public event Action<ConfigBase, List<Guid>> SubmitData = delegate { };

        /// <summary>
        ///     The GameMaster class represents the game master that manages the Tic Tac Toe game.
        /// </summary>
        public GameMaster(ILogger logger)
        {
            _logger = logger;
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
            if (session.Item1)
            {
                session.Item2!.HandleAction(message);
            }
            else
            {
                _logger.LogError($"Cannot find a session with a player with the specified id!");
            }
        }

        public void RestartSession(RestartMessage restartMessage)
        {
            GameSession session = _rooms.FirstOrDefault(r => r.GetPlayerManager().HasPlayer(restartMessage.ClientId));

            if (session != null)
            {
                session.Restart();
            }
        }

        public void ClientConnected(Guid clientId)
        {
            if (CheckForAvalibleSession())
            {
                GameSession gameSession = _rooms.First(r => !r.IsFull);

                gameSession.AddPlayer(clientId);

                SendInitialSessionData(gameSession);
            }
            else
            {
                GameSession gameSession = StartNewGameSession();
                gameSession.MessageProcessed += On_SessionDataProcessed;
                gameSession.AddPlayer(clientId);
            }
        }

        private void SendInitialSessionData(GameSession gameSession)
        {
            if (gameSession.IsFull)
            {
                List<Guid> playersIds = gameSession.GetSessionPlayers();
                NewSessionConfig config = gameSession.GetStartSessionData();

                SubmitData?.Invoke(config, playersIds);
            }
        }

        public void OnMessageRecived(MessageBase message)
        {
            message.Handle(this);
        }

        private (bool, GameSession) FindSessionByPlayerId(Guid playerId)
        {
            foreach (var room in _rooms)
            {
                if (room.GetPlayerManager().HasPlayer(playerId))
                {
                    return (true, room);
                }
            }

            return (false, null);
        }

        private void On_SessionDataProcessed(object sender, ConfigBase config)
        {
            GameSession gameSession = (GameSession)sender;

            List<Guid> playersIds = gameSession.GetSessionPlayers();
            SubmitData?.Invoke(config, playersIds);
        }

        private bool CheckForAvalibleSession() => _rooms.Any(r => r.IsFull == false);
    }
}