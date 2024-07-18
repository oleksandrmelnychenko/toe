namespace Tic_tac_toe_Server.Net.Strategies
{
    public class MessageStrategyFactory
    {
        public static IMessageStrategy GetStrategy(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.NewAction => new NewActionStrategy(),
                MessageType.Restart => new RestartStrategy(),
                MessageType.PlayerInitialized => new PlayerInitializedStrategy(),
                _ => throw new Exception($"Unsupported message Type: {messageType}")
            };
        }
    }
}
