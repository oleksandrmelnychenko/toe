using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Net.Messages
{
    public class RestartMessage : MessageBase
    {
        public Guid ClientId { get; set; }
    }
}
