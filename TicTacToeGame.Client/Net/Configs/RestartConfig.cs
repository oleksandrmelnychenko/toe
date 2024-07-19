using Newtonsoft.Json;
using System;

namespace TicTacToeGame.Client.Net.Configs
{
    public record RestartConfig(Guid ClientId) : ConfigBase
    {
        [JsonProperty]
        Messages.MessageType Type = Messages.MessageType.Restart;
    }
}
