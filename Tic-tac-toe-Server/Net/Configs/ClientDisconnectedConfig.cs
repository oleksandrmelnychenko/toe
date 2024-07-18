using Newtonsoft.Json;
using Tic_tac_toe_Server.Net.Messages;

namespace Tic_tac_toe_Server.Net.Configs
{
    public record ClientDisconnectedConfig(Guid ClientId) : ConfigBase
    {
        [JsonProperty]
        MessageType Type = MessageType.ClientDisconnected;
    }
}
