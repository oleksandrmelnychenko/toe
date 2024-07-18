
using Newtonsoft.Json;
using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Net.Messages
{
    public record NewSessionConfig(SessionStatus status, Guid CurrentPlayerId, Symbol CurrentPlayerSymbol) : ConfigBase
    {
        [JsonProperty("Type")]
        MessageType type = MessageType.NewGameSession;
    }
}
