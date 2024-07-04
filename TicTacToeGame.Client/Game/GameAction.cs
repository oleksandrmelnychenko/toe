using System;
using TicTacToeGame.Client.Models;

namespace TicTacToeGame.Client.Game;

public record GameAction(User User, int CellIndex)
{
    public DateTime ActionTime { get; set; } = DateTime.Now;

    public Guid Id { get; set; } = Guid.NewGuid();
}
