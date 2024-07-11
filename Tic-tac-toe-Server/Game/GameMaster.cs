using Tic_tac_toe_Server.Player;
using TicTacToeGame.Client.Game;

namespace Tic_tac_toe_Server.Game
{
    public class GameMaster
    {
        private PlayerManager _playerManager = new(2);

        private List<GameSession> _rooms;

        /// <summary>
        ///     The GameMaster class represents the game master that manages the Tic Tac Toe game.
        /// </summary>
        public GameMaster()
        {

        }

        /// <summary>
        ///     Starts a new Tic Tac Toe game session.
        /// </summary>
        public void StartNewGameSession()
        {
            _rooms.Add(new GameSession());
        }

        public void NewAction(BoardCell boardCell, Guid gameSessionId)
        {
            ArgumentNullException.ThrowIfNull(nameof(_rooms));
            GameSession room = _rooms!.FirstOrDefault(r => r.Id == gameSessionId);
            ArgumentNullException.ThrowIfNull(nameof(room));
            GameAction action = new(room!.GetCurrentPlayer(), boardCell.Index);

            room!.HandleAction(action);
        }

        //public PlayerManager GetPlayerService()
        //{
        //    ArgumentNullException.ThrowIfNull(nameof(_rooms));
        //    return _rooms!.GetPlayerService();
        //}
    }
}