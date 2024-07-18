﻿using Newtonsoft.Json;
using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Net.Messages
{
    public record NewGameDataConfig(SessionStatus status, Symbol CellSymbol, Symbol CurrentPlayerSymbol, ushort CellIndex, Guid CurrentPlayerId, string ActionHistory) : ConfigBase
    {
        [JsonProperty]
        MessageType Type = MessageType.NewGameData;
    }
}
