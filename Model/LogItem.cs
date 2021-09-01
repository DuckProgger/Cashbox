using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model
{
    public class LogItem
    {
        public DateTime Date { get; set; }
        public string Workers { get; set; }
        public int TotalCash { get; set; }
        public int Difference { get; set; }
        public string User { get; set; }
    }
}
