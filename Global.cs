using Cashbox.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox
{
    public static class Global
    {
        public static Session Session { get; private set; }

        public static void InitSession(string userName) => Session = DB.CreateSession(userName);

        public static int CalculateSalary(List<Shift> shifts)
        {
            const double Percent = 0.075;
            const int minDailySalary = 1000;

            double salary = 0;
            double dailySalary;

            foreach (var shift in shifts)
            {
                dailySalary = shift.Total * Percent;
                if (dailySalary < minDailySalary)
                    dailySalary = minDailySalary;
                salary += dailySalary;
            }
            return (int)Math.Ceiling(salary);
        }
    }
}
