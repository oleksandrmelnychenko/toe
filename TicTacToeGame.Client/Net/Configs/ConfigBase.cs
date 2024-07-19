using TicTacToeGame.Client.Net.Messages;

namespace TicTacToeGame.Client.Net.Configs
{
    public abstract record ConfigBase
    {
        MessageType Type { get; set; }
    }
}
