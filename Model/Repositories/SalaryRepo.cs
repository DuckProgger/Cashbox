using Cashbox.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model.Repositories
{
    public static class SalaryRepo
    {
        public static List<Salary> GetSalaries(int workerId, DateTime start, DateTime end)
        {
            using ApplicationContext db = new();
            return (from s in db.Salaries
                    where s.WorkerId == workerId && s.StartPeriod >= start.Date && s.EndPeriod <= end.Date
                    select s).ToList();
        }

        public static List<Salary> GetSalaries(DateTime start, DateTime end)
        {
            using ApplicationContext db = new();
            return (from s in db.Salaries
                    where s.StartPeriod >= start.Date && s.EndPeriod <= end.Date
                    select s).ToList();
        }

        public static int GetTotalSalary(int workerId, DateTime start, DateTime end)
        {
            using ApplicationContext db = new();
            return (from s in db.Salaries
                    where s.StartPeriod >= start.Date && s.EndPeriod <= end.Date && s.WorkerId == workerId
                    select s.Money).Sum();
        }

        public static int GetTotalSalary(DateTime start, DateTime end)
        {
            using ApplicationContext db = new();
            return (from s in db.Salaries
                    where s.StartPeriod >= start.Date && s.EndPeriod <= end.Date
                    select s.Money).Sum();
        }
    }
}
