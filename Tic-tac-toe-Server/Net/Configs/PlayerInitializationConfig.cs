using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_tac_toe_Server.Net.Messages
{
    public record PlayerInitializationConfig(Guid PlayerId) : ConfigBase
    {
        [JsonProperty]
        Type Type = Type.PlayerInitialization;
    }
}
