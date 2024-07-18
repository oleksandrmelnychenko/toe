namespace Tic_tac_toe_Server.Net.Messages
{
    public class ClientDisconnectedMessage : MessageBase
    {
        public Guid ClientId { get; set; }
    }
}
