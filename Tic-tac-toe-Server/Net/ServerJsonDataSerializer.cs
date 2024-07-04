using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using Tic_tac_toe_Server.Net;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Models;

namespace TicTacToeGame.Client.Net
{
    public static class ServerJsonDataSerializer
    {
        public static string SerializeMove(ClientGameMessage move)
        {
            return JsonConvert.SerializeObject(move);
        }

        public static ClientGameMessage DeserializeMove(string move)
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

        public static ServerGameMessage DeserializeServerMessage(string serverMessage)
        {
            if (string.IsNullOrEmpty(serverMessage))
            {
                Debug.WriteLine($"Null data recived.");
                return null;
            }
            return JsonConvert.DeserializeObject<ServerGameMessage>(serverMessage)!;
        }

        public static string SerializeUser(Player user)
        {
            return JsonConvert.SerializeObject(user);
        }
    }
}
