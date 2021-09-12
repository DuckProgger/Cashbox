using Cashbox.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cashbox.Visu.Converters
{
    public class DateConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return Formatter.FormatDate(date);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
