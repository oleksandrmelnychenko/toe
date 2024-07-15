using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Messages
{
    public record NewGameDataConfig(Status status, List<Symbol> Board, Guid CurrentPlayerId) : MessageBase
    {
        Type type = Type.NewGameData;
    }
}
