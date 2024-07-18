﻿using Newtonsoft.Json;

namespace Tic_tac_toe_Server.Net.Messages
{
    public record PlayerInitializationConfig(Guid PlayerId) : ConfigBase
    {
        [JsonProperty]
        MessageType Type = MessageType.PlayerInitialization;
    }
}
