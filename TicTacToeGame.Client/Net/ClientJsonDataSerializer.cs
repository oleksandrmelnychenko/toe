using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Tic_tac_toe_Server.Net;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Models;

namespace TicTacToeGame.Client.Net
{
    public static class ClientJsonDataSerializer
    {
        public static string SerializeAction(ClientGameMessage move)
        {
            return JsonConvert.SerializeObject(move);
        }

        public static ServerGameMessage DeserializeServerMessage(string serverMessage)
        {
            if (string.IsNullOrEmpty(serverMessage))
            {
                Debug.WriteLine($"Null data recived.");
                return null;
            }
            return JsonConvert.DeserializeObject<ServerGameMessage>(serverMessage)!;
        }

        public static Player DeserializePlayer(string playerMessage)
        {
            if (string.IsNullOrEmpty(playerMessage))
            {
                Debug.WriteLine($"Null data recived.");
                return null;
            }
            return JsonConvert.DeserializeObject<Player>(playerMessage)!;
        }
    }
}
