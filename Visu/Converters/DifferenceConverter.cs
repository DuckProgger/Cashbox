using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cashbox.Visu.Converters
{
    public class DifferenceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Abs((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
