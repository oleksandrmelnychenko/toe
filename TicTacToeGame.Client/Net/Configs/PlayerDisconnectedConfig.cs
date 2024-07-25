using Newtonsoft.Json;
using System;

namespace TicTacToeGame.Client.Net.Configs
{
    public record PlayerDisconnectedConfig(Guid ClientId) : ConfigBase
    {
        [JsonProperty]
        Messages.MessageType Type = Messages.MessageType.PlayerDisconnected;
    }
}
