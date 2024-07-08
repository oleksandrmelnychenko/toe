using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;
using TicTacToeGame.Client.Symbols;

namespace TicTacToeGame.Client.Game
{
    public class SymbolToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Symbol symbol)
            {
                switch (symbol)
                {
                    //"C:\\Projects\\Tic-tac-toe\\TicTacToeGame.Client\\Symbols\\X.png");
                    case Symbol.X:
                        return new Bitmap(SymbolPath.XPath);
                    case Symbol.O:
                        return new Bitmap(SymbolPath.OPath);
                    default:
                        return new Bitmap(SymbolPath.EmptyPath);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}