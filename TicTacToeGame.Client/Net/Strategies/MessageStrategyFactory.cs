using System;
using TicTacToeGame.Client.Net.Messages;
using TicTacToeGame.Client.Net.Strategies;

namespace Tic_tac_toe_Server.Net.Strategies
{
    public class MessageStrategyFactory
    {
        public static IMessageStrategy GetStrategy(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.PlayerInitialization => new ClientInitializationStrategy(),
                MessageType.NewGameData => new NewGameDataStrategy(),
                MessageType.NewGameSession => new NewGameSessionStrategy(),
                _ => throw new Exception($"Unsupported message Type: {messageType}")
            };
        }
    }
}
