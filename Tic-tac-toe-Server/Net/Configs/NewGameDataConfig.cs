using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Net.Messages
{
    public record NewGameDataConfig(Status status, Symbol CellSymbol, ushort CellIndex, Guid CurrentPlayerId) : ConfigBase
    {
        Type type = Type.NewGameData;
    }
}
