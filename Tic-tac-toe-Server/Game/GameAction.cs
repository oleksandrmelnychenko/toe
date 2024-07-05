using Tic_tac_toe_Server.Player;

namespace Tic_tac_toe_Server.Game
{
    public record GameAction(PlayerBase Player, int BoxPosition)
    {
        public DateTime ActionTime { get; set; } = DateTime.Now;

        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
