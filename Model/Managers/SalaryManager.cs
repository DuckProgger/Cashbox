﻿using Cashbox.Exceptions;
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
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static int GetTotalSalary(string workerName, DateTime startPeriod, DateTime endPeriod)
        {
            int workerId = DB.GetWorker(workerName)?.Id ?? throw new InvalidNameException("Работник не найден");
            return DB.GetTotalSalary(workerId, startPeriod, endPeriod);
        }

        public static int GetTotalSalary(DateTime startPeriod, DateTime endPeriod)
        {
            return DB.GetTotalSalary(startPeriod, endPeriod);
        }

        public static List<SalaryViewItem> GetSalaryLog(DateTime startPeriod, DateTime endPeriod, bool combine)
        {
            List<Salary> salariesPerPeriod = DB.GetSalaries(startPeriod, endPeriod);
            return combine ? GetSalaryLogWithCombine(salariesPerPeriod) : GetSalaryLogWithoutCombine(salariesPerPeriod);
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

        private static List<SalaryViewItem> GetSalaryLogWithCombine(List<Salary> salaries)
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
    }
}