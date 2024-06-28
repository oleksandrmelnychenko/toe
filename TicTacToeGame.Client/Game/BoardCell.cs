namespace TicTacToeGame.Client.Game;

public record BoardCell(int Index)
{
    public string? Value { get; set; }

    public bool IsDirty { get; set; }
}