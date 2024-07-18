using Newtonsoft.Json;
using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Net.Messages;

namespace Tic_tac_toe_Server.Net.Configs
{
    internal record RestartConfig(SessionStatus status, Guid CurrentPlayerId, Symbol CurrentPlayerSymbol) : ConfigBase
    {
        [JsonProperty("Type")]
        MessageType type = MessageType.Restart;
    }
}
