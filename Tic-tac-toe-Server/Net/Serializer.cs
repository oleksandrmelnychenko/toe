using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tic_tac_toe_Server.Net.Messages;
using TicTacToeGame.Client.Net.Messages.ToGameMessages;

namespace Tic_tac_toe_Server.Net
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
                        Type.NewAction =>
                            JsonConvert.DeserializeObject<NewActionMessage>(json)!,
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

        public static JsonValidationResult SerializeClientData(Guid id)
        {
            try
            {
                PlayerInitializationConfig config = new PlayerInitializationConfig(id);
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

        public static JsonValidationResult SerializeNewSession(NewSessionConfig config)
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
