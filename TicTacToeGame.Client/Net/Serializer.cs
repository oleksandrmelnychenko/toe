using System.Text.Json;
using System;
using TicTacToeGame.Client.Net.Messages;
using Type = TicTacToeGame.Client.Net.Messages.Type;
using TicTacToeGame.Client.Net.Messages.ToGameMessages;
using TicTacToeGame.Client.Net.Messages.ToClientMessages;

namespace TicTacToeGame.Client.Net
{
    internal static class Serializer
    {
        public static Messages.ToGameBaseMessage ParseMessage(string json)
        {
            var jsonObject = JsonDocument.Parse(json).RootElement;
            bool parseResult = jsonObject.TryGetProperty("Type", out JsonElement typeElement);

            if (typeElement.ValueKind == JsonValueKind.Number)
            {
                Type messageType = (Type)typeElement.GetInt32();

                return messageType switch
                {
                    Type.PlayerInitialization => JsonSerializer.Deserialize<ClientInitializationMessage>(json)!,
                    Type.NewGameSession => JsonSerializer.Deserialize<NewGameSessionMessage>(json)!,
                    Type.NewGameData => JsonSerializer.Deserialize<NewGameDataMessage>(json)!,
                    _ => throw new Exception()
                };
            }
            else
            {
                throw new InvalidOperationException($"Invalid message type: {json}");
            }
        }
    }
}
