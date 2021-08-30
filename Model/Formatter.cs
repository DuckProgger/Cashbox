﻿using System;
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
    }
}
