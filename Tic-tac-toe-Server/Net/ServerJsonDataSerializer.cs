using Newtonsoft.Json;
using System.Diagnostics;
using Tic_tac_toe_Server.Player;
using TicTacToeGame.Client.Net;

namespace Tic_tac_toe_Server.Net
{
    public static class ServerJsonDataSerializer
    {
        public static ClientToServerConfig DeserializeAction(string action)
        {
            if (string.IsNullOrEmpty(action))
            {
                Debug.WriteLine($"Null data recived.");
                return null;
            }
            return JsonConvert.DeserializeObject<ClientToServerConfig>(action)!;
        }

        public static string SerializeServerMessage(ServerToClientConfig serverToClientConfig)
        {
            return JsonConvert.SerializeObject(serverToClientConfig);
        }

        public static string SerializePlayerId(Guid userId)
        {
            return JsonConvert.SerializeObject(userId);
        }
    }
}
