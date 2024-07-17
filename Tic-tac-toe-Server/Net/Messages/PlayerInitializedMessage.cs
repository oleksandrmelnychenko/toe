using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Net.Messages
{
    public class PlayerInitializedMessage : MessageBase
    {
        public Guid ClientId { get; set; }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.ClientConnected(ClientId);
        }
    }
}
