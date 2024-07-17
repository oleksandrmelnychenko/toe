using Newtonsoft.Json;
using Tic_tac_toe_Server.Game;
using Tic_tac_toe_Server.Net.Messages;

namespace Tic_tac_toe_Server.Net.Configs
{
    internal record RestartConfig(Status status, Guid CurrentPlayerId, Symbol CurrentPlayerSymbol) : ConfigBase
    {
        [JsonProperty("Type")]
        Type type = Type.Restart;
    }
}
