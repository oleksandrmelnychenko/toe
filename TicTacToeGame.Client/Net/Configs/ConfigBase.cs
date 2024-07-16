using TicTacToeGame.Client.Net.Messages;

namespace TicTacToeGame.Client.Net.Configs
{
    public abstract record ConfigBase
    {
        Type Type { get; set; }
    }
}
