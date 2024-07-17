using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace TicTacToeGame.Client.Net.Configs
{
    internal record PlayerInitializedConfig(Guid ClientId)
    {
        [JsonProperty]
        Messages.Type Type = Messages.Type.PlayerInitialized;
    }
}
