namespace TicTacToeGame.Client.Net.Messages
{
    public enum MessageType
    {
        PlayerInitialization,
        NewGameSession,
        NewGameData,
        NewAction,
        Restart,
        PlayerInitialized,
        PlayerDisconnected
    }
}
