using Newtonsoft.Json;
using System;

namespace TicTacToeGame.Client.Net.Configs
{
    internal record PlayerInitializedConfig(Guid ClientId) : ConfigBase
    {
        [JsonProperty]
        Messages.MessageType Type = Messages.MessageType.PlayerInitialized;
    }
}
