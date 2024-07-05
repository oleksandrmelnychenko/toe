using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Tic_tac_toe_Server.Net;
using TicTacToeGame.Client.Game;
using Tic_tac_toe_Server.Player;

namespace TicTacToeGame.Client.Net
{
    public static class ServerJsonDataSerializer
    {
        public static ClientGameMessage DeserializePlayer(string move)
        {
            if (string.IsNullOrEmpty(move))
            {
                Debug.WriteLine($"Null data recived.");
                return null;
            }
            return JsonConvert.DeserializeObject<ClientGameMessage>(move)!;
        }

        public static string SerializeServerMessage(ServerGameMessage serverGameMessage)
        {
            return JsonConvert.SerializeObject(serverGameMessage);
        }

        public static string SerializePlayer(PlayerBase user)
        {
            return JsonConvert.SerializeObject(user);
        }
    }
}
