namespace Tic_tac_toe_Server.Net.Messages
{
    public class PlayerDisconnectedMessage : MessageBase
    {
        public Guid ClientId { get; set; }
    }
}
