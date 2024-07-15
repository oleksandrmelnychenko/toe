
using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Messages
{
    public record NewSessionConfig(Status status, List<Symbol> Board, Guid CurrentPlayerId) : MessageBase
    {
        Type type = Type.NewGameData;
    }
}
