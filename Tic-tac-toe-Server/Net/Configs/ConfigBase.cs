namespace Tic_tac_toe_Server.Net.Messages
{
    public abstract record ConfigBase
    {
        MessageType Type { get; set; }
    }
}
