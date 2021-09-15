﻿using Cashbox.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Cashbox.Visu.Converters
{
    public class DateConverterToFirstDay : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return Formatter.ReturnToFirstDay(date);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            return Formatter.ReturnToFirstDay(date);
        }
    }
}
