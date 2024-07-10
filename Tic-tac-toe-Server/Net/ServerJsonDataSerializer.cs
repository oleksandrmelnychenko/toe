using Newtonsoft.Json;
using System.Diagnostics;
using Tic_tac_toe_Server.Logging;
using TicTacToeGame.Client.Net;

namespace Tic_tac_toe_Server.Net
{
    public static class ServerJsonDataSerializer
    {
        private static readonly ConsoleLogger _logger = new(); 

        public static ClientToServerConfig DeserializeAction(string action)
        {
            try
            {
                ClientToServerConfig config = JsonConvert.DeserializeObject<ClientToServerConfig>(action);
                if (config == null)
                {
                    throw new InvalidOperationException("Deserialization resulted in null.");
                }
                return config;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON deserialization error: {ex.Message}");
                return default;
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"Deserialization error: {ex.Message}");
                return default;
            }
        }

        public static string SerializeServerMessage(ServerToClientConfig serverToClientConfig)
        {
            try
            {
                return JsonConvert.SerializeObject(serverToClientConfig);
            }
            catch(JsonException ex)
            {
                _logger.LogError($"JSON serealization problem: {ex}");
                return string.Empty;
            }
            catch(Exception ex)
            {
                _logger.LogError($"Serealization problem: {ex}");
                return string.Empty;
            }
        }

        public static string SerializePlayerId(Guid userId)
        {
            try
            {
                return JsonConvert.SerializeObject(userId);
            }
            catch (JsonException ex)
            {
                _logger.LogError($"JSON serealization problem: {ex}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Serealization problem: {ex}");
                return string.Empty;
            }
        }
    }
}
