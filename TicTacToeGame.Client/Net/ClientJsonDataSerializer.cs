using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace TicTacToeGame.Client.Net
{
    public static class ClientJsonDataSerializer
    {
        public static string SerializeAction(ClientToServerConfig config)
        {
            try
            {
                return JsonConvert.SerializeObject(config);
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON serealization problem: {ex}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Serealization problem: {ex}");
                return string.Empty;
            }
        }

        public static ServerToClientConfig DeserializeServerMessage(string serverMessage)
        {
            try
            {
                ServerToClientConfig config = JsonConvert.DeserializeObject<ServerToClientConfig>(serverMessage);
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

        public static Guid DeserializePlayerId(string playerMessage)
        {
            try
            {
                Guid id = JsonConvert.DeserializeObject<Guid>(playerMessage);
                if (id == Guid.Empty)
                {
                    throw new InvalidOperationException("Deserialization result is empty.");
                }
                return id;
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON deserialization error: {ex.Message}");
                return Guid.Empty;
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"Deserialization error: {ex.Message}");
                return Guid.Empty;
            }
        }
    }
}
