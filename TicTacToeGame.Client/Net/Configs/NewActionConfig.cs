using Newtonsoft.Json;
using System;

namespace TicTacToeGame.Client.Net.Configs
{
    public record NewActionConfig(ushort CellIndex, Guid ClientId) : ConfigBase
    {
        [JsonProperty]
        Messages.Type Type = Messages.Type.NewAction;
    }
}
