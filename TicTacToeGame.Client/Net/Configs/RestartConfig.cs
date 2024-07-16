using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeGame.Client.Net.Configs
{
    public record RestartConfig(Guid ClientId)
    {
        [JsonProperty]
        Messages.Type Type = Messages.Type.Restart;
    }
}
