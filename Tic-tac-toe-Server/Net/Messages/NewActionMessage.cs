using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Net.Messages
{
    public class NewActionMessage : MessageBase
    {
        public Guid ClientId { get; set; }

        public ushort CellIndex { get; set; }

        public override void Handle(GameMaster gameMaster)
        {
            gameMaster.NewAction(this);
        }
    }
}
