namespace Tic_tac_toe_Server.Player
{
    public abstract class PlayerBase
    {
        public Guid Id { get; set; }

        public string PlayerSymbolName { get; set; }

        public bool IsActived { get; set; }
    }
}
