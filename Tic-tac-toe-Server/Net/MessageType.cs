namespace Tic_tac_toe_Server.Net
{
    public enum MessageType
    {
        PlayerInitialization,
        NewGameSession,
        NewGameData,
        NewAction,
        Restart,
        PlayerInitialized,
        ClientDisconnected
    }
}
