
using Newtonsoft.Json;
using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Net.Messages
{
    public record NewSessionConfig(Status status, Guid CurrentPlayerId, Symbol CurrentPlayerSymbol) : ConfigBase
    {
        [JsonProperty("Type")]
        Type type = Type.NewGameSession;
    }
}
