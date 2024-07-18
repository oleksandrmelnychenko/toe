namespace Tic_tac_toe_Server.Net.Messages
{
    public class NewActionMessage : MessageBase
    {
        public Guid ClientId { get; set; }

        public ushort CellIndex { get; set; }
    }
}
