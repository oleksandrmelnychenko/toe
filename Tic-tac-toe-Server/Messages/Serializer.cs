using Newtonsoft.Json;

namespace Tic_tac_toe_Server.Messages
{
    internal static class Serializer
    {
        public static JsonValidationResult SerializeClientData(Guid id)
        {
            try
            {
                string json = JsonConvert.SerializeObject(id);
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
