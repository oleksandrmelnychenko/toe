using System;
using TicTacToeGame.Client.Models;

namespace Tic_tac_toe_Server.Game
{
    public record GameAction(Player Player, int BoxPosition)
    {
        public DateTime ActionTime { get; set; } = DateTime.Now;

        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
