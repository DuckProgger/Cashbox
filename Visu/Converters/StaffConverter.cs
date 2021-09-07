using Cashbox.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Cashbox.Visu.Converters
{
    public class StaffConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<Worker> workers = value as List<Worker>;
            StringBuilder builder = new();
            for (int i = 0; i < workers.Count; i++)
            {
                builder.Append(workers[i].Name);
                if (i < workers.Count - 1)
                    builder.Append(',');
            }
            return builder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
