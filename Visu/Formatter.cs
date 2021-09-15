using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model
{
    public class Formatter
    {
        public enum Type { MoneyEnter }

        public static string Format(string str, Type type)
        {
            return type switch
            {
                Type.MoneyEnter => FormatMoneyEnter(str),
                _ => string.Empty,
            };
        }

        public static string AddZero(int value) => value < 10 ? '0' + value.ToString() : value.ToString();

        private static string FormatMoneyEnter(string str)
        {
            if (str?.Length == 0)
                str = "0";
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsDigit(str[i]))
                    str = str.Remove(i, 1);
            }
            return str;
        }

        public static string FormatDate(DateTime date) => $"{AddZero(date.Day)}.{AddZero(date.Month)}.{date.Year}";

        public static string FormatDatePeriod(DateTime start, DateTime end) => $"{start.Day} - {end.Date.Day} {FormatMonth(start.Month)}";

        public static string FormatWithoutDay(DateTime date) => $"{FormatMonth(date.Month)} {date.Year}";

        public static DateTime ReturnToFirstDay(DateTime date) => date.AddDays(-date.Day + 1);

        public static DateTime ReturnToMiddleOfMonth(DateTime date)
        {
            date = ReturnToFirstDay(date);
            date = date.AddDays(14);
            return date;
        }

        public static DateTime ReturnToEndOfMonth(DateTime date)
        {
            date = ReturnToFirstDay(date);
            date = date.AddMonths(1);
            date = date.AddDays(-1);
            return date;
        }

        private static string FormatMonth(int monthNum)
        {
            return monthNum switch
            {
                1 => "янв",
                2 => "фев",
                3 => "мар",
                4 => "апр",
                5 => "май",
                6 => "июн",
                7 => "июл",
                8 => "авг",
                9 => "сен",
                10 => "окт",
                11 => "ноя",
                12 => "дек",
                _ => "",
            };
        }
    }
}
