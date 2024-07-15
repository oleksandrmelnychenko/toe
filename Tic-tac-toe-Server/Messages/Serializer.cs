using Newtonsoft.Json;

namespace Tic_tac_toe_Server.Messages
{
    internal static class Serializer
    {
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
