namespace Tic_tac_toe_Server.Net.Messages
{
    public class RestartMessage : MessageBase
    {
        public Guid ClientId { get; set; }
    }
}
