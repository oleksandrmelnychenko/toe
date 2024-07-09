using Tic_tac_toe_Server.Game;

namespace Tic_tac_toe_Server.Player
{
    public class Player : PlayerBase
    {
        public Player(Symbol symbolName, bool isActive, PlayerStatus status)
        {
            PlayerSymbolName = symbolName;
            Id = Guid.NewGuid();
            IsActived = isActive;
            Status = status;
        }
    }
}
