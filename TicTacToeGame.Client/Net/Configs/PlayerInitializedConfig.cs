using Newtonsoft.Json;
using System;

namespace TicTacToeGame.Client.Net.Configs
{
    internal record PlayerInitializedConfig(Guid ClientId)
    {
        [JsonProperty]
        Messages.Type Type = Messages.Type.PlayerInitialized;
    }
}
