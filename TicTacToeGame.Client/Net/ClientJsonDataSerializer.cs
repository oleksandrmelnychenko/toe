using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace TicTacToeGame.Client.Net
{
    public static class ClientJsonDataSerializer
    {
        public static string SerializeAction(ClientToServerConfig config)
        {
            return JsonConvert.SerializeObject(config);
        }

        public static ServerToClientConfig DeserializeServerMessage(string serverMessage)
        {
            if (string.IsNullOrEmpty(serverMessage))
            {
                Debug.WriteLine($"Null data recived.");
                return null;
            }
            return JsonConvert.DeserializeObject<ServerToClientConfig>(serverMessage)!;
        }

        public static Guid DeserializePlayerId(string playerMessage)
        {
            if (string.IsNullOrEmpty(playerMessage))
            {
                Debug.WriteLine($"Null data recived.");
                return Guid.Empty;
            }
            return JsonConvert.DeserializeObject<Guid>(playerMessage)!;
        }
    }
}
