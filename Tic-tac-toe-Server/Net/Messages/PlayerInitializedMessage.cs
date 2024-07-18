using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Net.Messages
{
    public class PlayerInitializedMessage : MessageBase
    {
        public Guid ClientId { get; set; }
    }
}
