using System;
using Type = TicTacToeGame.Client.Net.Messages.Type;
using TicTacToeGame.Client.Net.Messages.ToGameMessages;
using TicTacToeGame.Client.Net.Messages.ToClientMessages;
using TicTacToeGame.Client.Net.Messages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using TicTacToeGame.Client.Net.Configs;

namespace TicTacToeGame.Client.Net
{
    internal static class Serializer
    {
        public static MessageBase ParseMessage(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            if (jsonObject.TryGetValue("Type", out JToken typeToken))
            {
                if (typeToken.Type == JTokenType.Integer)
                {
                    int typeInt = typeToken.ToObject<int>();
                    Type messageType = (Type)typeInt;

                    return messageType switch
                    {
                        Type.PlayerInitialization =>
                            JsonConvert.DeserializeObject<ClientInitializationMessage>(json)!,
                        Type.NewGameSession =>
                            JsonConvert.DeserializeObject<NewGameSessionMessage>(json)!,
                        Type.NewGameData =>
                            JsonConvert.DeserializeObject<NewGameDataMessage>(json)!,
                        _ => throw new Exception($"Unsupported message type: {typeInt}")
                    };
                }
                else
                {
                    throw new InvalidOperationException($"Invalid message type: {json}");
                }
            }
            else
            {
                throw new InvalidOperationException($"Missing 'Type' property in JSON: {json}");
            }
        }

        public static JsonValidationResult SerializeNewAction(NewActionConfig config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config);
                return new JsonValidationResult(true, json);
            }
            catch (JsonException ex)
            {
                return new JsonValidationResult(false, $"Serialization error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new JsonValidationResult(false, $"Unexpected error: {ex.Message}");
            }
        }

    }
}
