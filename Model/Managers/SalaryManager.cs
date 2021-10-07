using Cashbox.Exceptions;
using Cashbox.Model.Entities;
using Cashbox.Visu.ViewEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model.Managers
{
    public class SalaryManager : INotifyPropertyChanged
    {
        private int _totalSalary;

        public int TotalSalary
        {
            get => _totalSalary;
            set { _totalSalary = value; OnPropertyChanged(); }
        }

        //public ObservableCollection<SalaryViewItem> SalaryLog { get; set; } = new();

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static List<SalaryViewItem> GetSalaryLog(DateTime startPeriod, DateTime endPeriod, bool combine)
        {
            //SalaryLog.Clear();

            List<Salary> salariesPerPeriod = DB.GetSalaries(startPeriod, endPeriod);
            if (combine) // объединять в месяц
                return FillSalaryLogWithCombine(salariesPerPeriod);
            else
                return GetSalaryLogWithoutCombine(salariesPerPeriod);
        }

        public static List<SalaryViewItem> GetSalaryLog(string workerName, DateTime startPeriod, DateTime endPeriod, bool combine)
        {
            if (!IsValidWorkerSelected(workerName))
                throw new InvalidNameException("Не выбран работник");

            //SalaryLog.Clear();

            Worker worker = DB.GetWorker(workerName);
            List<Salary> salariesPerPeriod = DB.GetSalaries(worker.Id, startPeriod, endPeriod);
            if (combine) // объединять в месяц
                return FillSalaryLogWithCombine(salariesPerPeriod);
            else
                return GetSalaryLogWithoutCombine(salariesPerPeriod);
        }

        private static List<SalaryViewItem> GetSalaryLogWithoutCombine(List<Salary> salaries)
        {
            // Так как объединять в месяц не нужно, то просто перебираем список зарплат
            // за выбранный период и выводим его на экран
            List<SalaryViewItem> salaryLog = new();
            foreach (var salary in salaries)
            {
                salaryLog.Add(new()
                {
                    Name = DB.GetWorker(salary.WorkerId).Name,
                    Salary = salary.Money,
                    Date = Formatter.FormatDatePeriod(salary.StartPeriod, salary.EndPeriod)
                });
            }
            return salaryLog;
        }

        private static List<SalaryViewItem> FillSalaryLogWithCombine(List<Salary> salaries)
        {
            List<SalaryViewItem> salaryLog = new();
            // Создать словарь, где ключом является Id работника,
            // а значением является список его смен за выбранный период
            Dictionary<int, List<Salary>> workersSalariesDict = new();

            // Сгруппировать список смен за выбранный период по Id работников
            var workersSalaries = from s in salaries
                                  group s by s.WorkerId;

            // Заполнить словарь этими группами. Ключ группы совпадает с ключом словаря.
            foreach (var workerSalariesGroup in workersSalaries)
                workersSalariesDict.Add(workerSalariesGroup.Key, workerSalariesGroup.ToList());

            // Сгруппировать смены каждого работника по месяцам, суммировать общий
            // заработок за месяц и вывести на экран
            foreach (var dictItem in workersSalariesDict)
            {
                foreach (var salaryItem in from s in dictItem.Value
                                           group s by s.StartPeriod.Month
                                            into sg
                                           select new SalaryViewItem()
                                           {
                                               Name = DB.GetWorker(dictItem.Key).Name, // ключ словаря - это Id работника
                                               Salary = sg.Sum(s => s.Money),
                                               Date = Formatter.FormatMonth(sg.Key) // получившийся ключ группы - это номер месяца
                                           })
                    salaryLog.Add(salaryItem);
            }
            return salaryLog;
        }

        private static bool IsValidWorkerSelected(string workerName) => workerName?.Length > 0;
    }
}