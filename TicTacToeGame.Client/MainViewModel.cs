using System.Collections.ObjectModel;
using Prism.Commands;
using ReactiveUI;
using TicTacToeGame.Client.Constants;
using TicTacToeGame.Client.Game;
using TicTacToeGame.Client.Models;

namespace TicTacToeGame.Client
{
    public sealed class MainViewModel : ReactiveObject
    {
        private string _gameStatusField;

        private ObservableCollection<BoardCell> _boardBoardCells;
        
        private readonly GameMaster _gameMaster = new();
        
        public ObservableCollection<BoardCell> BoardCells
        {
            get => _boardBoardCells;
            set => this.RaiseAndSetIfChanged(ref _boardBoardCells, value);
        }

        public string GameStatusField
        {
            get => _gameStatusField;
            set { this.RaiseAndSetIfChanged(ref _gameStatusField, value); }
        }

        public DelegateCommand<BoardCell> OnCellCommand { get; init; }

        public MainViewModel()
        {
            OnCellCommand = new DelegateCommand<BoardCell>(OnCellClickCommandHandler);

            _gameMaster.StartGame();

            BoardCells = new ObservableCollection<BoardCell>(_gameMaster.GetActiveGameSessionBoard());
            
            //GameStatusField = GameStatusConst.PlayerTurn + " " + _userService.CurrentUser.UserSymbolName;
            
            // GameStatusField = GameStatusConst.PlayerTurn + " " + _userService.CurrentUser.UserSymbolName;
        }

        private void OnCellClickCommandHandler(BoardCell boardCell)
        {
            /*BoardCells[int.Parse(param) - 1].BoxSetValues(_userService.CurrentUser.UserSymbol,
                _userService.CurrentUser.UserSymbolName);
            GameHistory.AddMove(new Move(_userService.CurrentUser, int.Parse(param) - 1));
            ChangeTurn();*/
        }

        /*public void ChangeTurn()
        {
            if (!_ticTacToeGameOutcomeDeterminer.IsWinner(boxCollection))
            {
                if (GameStatusField == GameStatusConst.PlayerTurn + " " + SymbolsConst.SymbolX)
                {
                    GameStatusField = GameStatusConst.PlayerTurn + " " + SymbolsConst.SymbolO;
                }
                else
                {
                    GameStatusField = GameStatusConst.PlayerTurn + " " + SymbolsConst.SymbolX;
                }
            }
            else
            {
                GameStatusField = GameStatusConst.EndOfGame + " " + _userService.CurrentUser.UserSymbolName;
                return;
            }

            if (TicTacToeGameOutcomeDeterminer.CheckForDraw(boxCollection))
            {
                GameStatusField = GameStatusConst.Draw;
            }

            _userService.ChangeCurrentUser();
        }*/
    }
}