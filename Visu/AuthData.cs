using Cashbox.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Visu
{
    public static class AuthData
    {
        public static Session Session { get; private set; }

        public static void InitSession(string userName) => Session = DB.CreateSession(userName);
    }
}
