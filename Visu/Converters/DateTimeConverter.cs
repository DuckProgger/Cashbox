using Cashbox.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cashbox.Visu.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return $"{Formatter.AddZero(date.Day)}.{Formatter.AddZero(date.Month)}.{date.Year} {Formatter.AddZero(date.Hour)}:{Formatter.AddZero(date.Minute)}:{Formatter.AddZero(date.Second)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
