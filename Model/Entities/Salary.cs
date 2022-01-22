using Cashbox.Exceptions;
using Cashbox.Model.Logging;
using Cashbox.Model.Logging.Entities;
using Cashbox.Model.Repositories;
using Cashbox.Visu.ViewEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Cashbox.Model.Entities
{
    public class Salary : ILogged
    {
        public int Id { get; set; }
        public int Money { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartPeriod { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndPeriod { get; set; }

        public int WorkerId { get; set; }
        public Worker Worker { get; set; }

        public ILogItem ConvertToLogItem() => (SalaryLogItem)this;

        public static Salary AddSalary(string workerName, DateTime startPeriod, DateTime endPeriod)
        {
            int workerId = WorkerRepo.GetWorker(workerName).Id;
            // проверка не выдана ли уже зарплата
            if (SalaryRepo.GetSalaries(workerId, startPeriod, endPeriod).Count > 0)
                throw new SalaryCountException($"Cотрудник {workerName} уже получал ЗП" +
                                               $" за период с {Formatter.FormatDate(startPeriod)} " +
                                               $"по {Formatter.FormatDate(endPeriod)}");
            Salary salary = CalculateSalary(workerName, startPeriod, endPeriod);
            Salary newSalary = CommonRepo.Create(salary);
            Logger.Log(newSalary, MessageType.Create);
            return newSalary;
        }

        public static Salary CalculateSalary(string workerName, DateTime startPeriod, DateTime endPeriod)
        {
            Worker worker = Worker.Get(workerName);

            const double Percent = 0.075;
            const int minDailySalary = 1000;

            double salary = 0;
            double dailySalary;

            foreach (Shift shift in Shift.GetShifts(workerName, startPeriod, endPeriod))
            {
                dailySalary = shift.Total * Percent;
                if (dailySalary < minDailySalary)
                    dailySalary = minDailySalary;
                salary += dailySalary;
            }
            return new Salary()
            {
                Money = (int)Math.Ceiling(salary),
                StartPeriod = startPeriod,
                EndPeriod = endPeriod,
                WorkerId = worker.Id
            };
        }

        public static int GetTotal(string workerName, DateTime startPeriod, DateTime endPeriod)
        {
            int workerId = Worker.Get(workerName).Id;
            return SalaryRepo.GetTotalSalary(workerId, startPeriod, endPeriod);
        }

        public static int GetTotal(DateTime startPeriod, DateTime endPeriod)
        {
            return SalaryRepo.GetTotalSalary(startPeriod, endPeriod);
        }

        public static List<SalaryViewItem> GetSalaryLog(DateTime startPeriod, DateTime endPeriod, bool combine)
        {
            List<Salary> salariesPerPeriod = SalaryRepo.GetSalaries(startPeriod, endPeriod);
            return combine ? GetSalaryLogWithCombine(salariesPerPeriod) : GetSalaryLogWithoutCombine(salariesPerPeriod);
        }

        private static List<SalaryViewItem> GetSalaryLogWithoutCombine(List<Salary> salaries)
        {
            // Так как объединять в месяц не нужно, то просто перебираем список зарплат
            // за выбранный период и выводим его на экран
            List<SalaryViewItem> salaryLog = new();
            foreach (Salary salary in salaries)
            {
                salaryLog.Add(new()
                {
                    Name = WorkerRepo.GetWorker(salary.WorkerId).Name,
                    Salary = salary.Money,
                    Date = Formatter.FormatDatePeriod(salary.StartPeriod, salary.EndPeriod)
                });
            }
            return salaryLog;
        }

        private static List<SalaryViewItem> GetSalaryLogWithCombine(List<Salary> salaries)
        {
            List<SalaryViewItem> salaryLog = new();
            foreach (KeyValuePair<int, List<Salary>> dictItem in GroupSalariesByWorkers(salaries))
                salaryLog.AddRange(GroupWorkerSalariesByMonths(dictItem));
            return salaryLog;
        }

        private static IEnumerable<SalaryViewItem> GroupWorkerSalariesByMonths(KeyValuePair<int, List<Salary>> workerSalaries)
        {
            // Сгруппировать смены каждого работника по месяцам,
            // суммировать общий заработок за месяц
            return from s in workerSalaries.Value
                   group s by s.StartPeriod.Month
                   into sg
                   select new SalaryViewItem()
                   {
                       Name = WorkerRepo.GetWorker(workerSalaries.Key).Name, // ключ словаря - это Id работника
                       Salary = sg.Sum(s => s.Money),
                       Date = Formatter.FormatMonth(sg.Key) // получившийся ключ группы - это номер месяца
                   };
        }

        private static Dictionary<int, List<Salary>> GroupSalariesByWorkers(List<Salary> salaries)
        {
            List<SalaryViewItem> salaryLog = new();
            // Создать словарь, где ключом является Id работника,
            // а значением является список его смен за выбранный период
            Dictionary<int, List<Salary>> workersSalariesDict = new();

            // Сгруппировать список смен за выбранный период по Id работников
            IEnumerable<IGrouping<int, Salary>> workersSalaries = from s in salaries
                                                                  group s by s.WorkerId;

            // Заполнить словарь этими группами. Ключ группы совпадает с ключом словаря.
            foreach (IGrouping<int, Salary> workerSalariesGroup in workersSalaries)
                workersSalariesDict.Add(workerSalariesGroup.Key, workerSalariesGroup.ToList());
            return workersSalariesDict;
        }
    }
}