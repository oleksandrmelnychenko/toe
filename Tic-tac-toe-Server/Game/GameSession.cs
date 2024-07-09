using System.Collections.Generic;
using TicTacToeGame.Client.Models;
using Tic_tac_toe_Server.Game;
using TicTacToeGame.Client.Game;
using Tic_tac_toe_Server.Player;

namespace Tic_tac_toe_Server.Game
{
    public class GameSession(List<BoardCell> boardCells)
    {
        private PlayerManager _playerService = new(2);

        //Тут змінив тип на ліст, тому що чомусь не оновлюється IReadOnlyCollection хоча ніби роблю посилання на один об'єкт і в гілці main це працює, можливо це через те що там ObservableCollection
        public List<BoardCell> BoardCells { get; private set; } = boardCells;

        public Status Status { get; set; } = Status.Start;


        public GameHistory History { get; set; } = new();

        public void HandleAction(GameAction action)
        {
            History.AddAction(action);
            UpdateGameStatus();
        }

        public PlayerBase GetCurrentPlayer()
        {
            return _playerService.CurrentPlayer;
        }


        public void UpdateGameStatus()
        {
            if (OutcomeDeterminer.IsWinner(BoardCells))
            {
                Status = Status.Finish;
            }
            else if (OutcomeDeterminer.IsDraw(BoardCells))
            {
                Status = Status.Draw;
            }
            else
            {
                Status = Status.PlayerTurn;
            }

            HandleGameStatus();
        }
        public PlayerManager GetPlayerService()
        {
            return _playerService;
        }

        private void HandleGameStatus()
        {
            if (Status == Status.PlayerTurn)
            {
                _playerService.ChangeCurrentPlayer();
            }
        }
    }
}
