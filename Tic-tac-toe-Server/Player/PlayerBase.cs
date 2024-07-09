using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Player
{
    public abstract class PlayerBase
    {
        public Guid Id { get; set; }

        public Symbol PlayerSymbolName { get; set; }

        public bool IsActived { get; set; }

        public PlayerStatus Status { get; set; }
    }
}
