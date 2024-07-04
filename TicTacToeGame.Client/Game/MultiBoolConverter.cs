using System;
using System.Globalization;
using Avalonia.Data.Converters;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToeGame.Client.Game
{
    public class MultiBoolToBoolConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Count < 2)
            {
                return false;
            }

            bool allTrue = values.All(value => value is bool b && b);
            return allTrue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
