using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Prism.Commands;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using Tic_tac_toe.Constants;
using Tic_tac_toe.Fabric;
using Tic_tac_toe.Models;
using Tic_tac_toe.Service;
using Tic_tac_toe.WinnerCombination;

namespace Tic_tac_toe.ViewModel
{
    internal partial class MainWindowViewModel : ViewModelBase
    {
        private string _gameStatusField;

        private EndOfGameChecker _endOfGameChecker;

        public GameHistory GameHistory { get; set; }

        private ObservableCollection<Cell> _cells;

        public ObservableCollection<Cell> Cells
        {
            get => _cells;
            set => this.RaiseAndSetIfChanged(ref _cells, value);
        }

        public string GameStatusField
        {
            get => _gameStatusField;
            set
            {
                this.RaiseAndSetIfChanged(ref _gameStatusField, value);
            }
        }

        private UserService _userService;
        public Cell[] boxCollection { get; set; }

        public DelegateCommand<string> BoxClickCommand { get; }
        public MainWindowViewModel(UserService userService, WinnerCombinationBase winnerCombination, GameHistory gameHistory)
        {
            BoxClickCommand = new DelegateCommand<string>(BoxClick);
            Cells = new ObservableCollection<Cell>(CellFactory.Build(9, CellType.Cell));
            _userService = userService;
            _endOfGameChecker = new EndOfGameChecker(winnerCombination);
            GameHistory = gameHistory;
            StartNewGame();
        }

        public void StartNewGame()
        {
            boxCollection = new Cell[9];
            for (int i = 0; i < boxCollection.Length; i++)
            {
                boxCollection[i] = new Cell();
            }
            GameStatusField = GameStatusConst.PlayerTurn + " " + _userService.CurrentUser.UserSymbolName;
        }

        public void RestartGame()
        {
            for (int i = 0; i < boxCollection.Length; i++)
            {
                boxCollection[i].BoxReset();
            }
            _userService.ChangeCurrentUser();
            GameStatusField = GameStatusConst.PlayerTurn + " " + _userService.CurrentUser.UserSymbolName;
        }

        public void BoxClick(string param)
        {
            Cells[int.Parse(param) - 1].BoxSetValues(_userService.CurrentUser.UserSymbol, _userService.CurrentUser.UserSymbolName);
            GameHistory.AddMove(new Move(_userService.CurrentUser, int.Parse(param) - 1));
            ChangeTurn();
        }

        public void ChangeTurn()
        {
            if (!_endOfGameChecker.CheckForWinner(boxCollection))
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

            if (_endOfGameChecker.CheckForDraw(boxCollection))
            {
                GameStatusField = GameStatusConst.Draw;
            }

            _userService.ChangeCurrentUser();
        }
    }
}
