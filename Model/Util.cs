using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model
{
   public class Util
    {
        public static T Cast<T>(object obj, T type) => (T)obj;
    }
}
