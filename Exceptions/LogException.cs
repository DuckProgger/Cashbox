using System;

namespace Cashbox.Exceptions
{
    internal class LogException : Exception
    {
        public LogException(string message) : base(message)
        {
        }
    }
}