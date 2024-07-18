using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tic_tac_toe_Server.Net.Messages;

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
                    MessageType messageType = (MessageType)typeInt;

                    return messageType switch
                    {
                        MessageType.NewAction =>
                            JsonConvert.DeserializeObject<NewActionMessage>(json)!,
                        MessageType.Restart =>
                            JsonConvert.DeserializeObject<RestartMessage>(json)!,
                        MessageType.PlayerInitialized =>
                           JsonConvert.DeserializeObject<PlayerInitializedMessage>(json)!,
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

        public static JsonValidationResult Serialize<T>(T config)
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
