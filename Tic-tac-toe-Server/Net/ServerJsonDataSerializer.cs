using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Tic_tac_toe_Server.Net;
using TicTacToeGame.Client.Game;
using Tic_tac_toe_Server.Player;
using TicTacToeGame.Client.Net;

namespace Tic_tac_toe_Server.Net
{
    public static class ServerJsonDataSerializer
    {
        public static ClientToServerConfig DeserializeAction(string move)
        {
            if (string.IsNullOrEmpty(move))
            {
                Debug.WriteLine($"Null data recived.");
                return null;
            }
            return JsonConvert.DeserializeObject<ClientToServerConfig>(move)!;
        }

        public static string SerializeServerMessage(ServerToClientConfig serverToClientConfig)
        {
            return JsonConvert.SerializeObject(serverToClientConfig);
        }

        public static string SerializePlayer(PlayerBase user)
        {
            return JsonConvert.SerializeObject(user);
        }
    }
}
