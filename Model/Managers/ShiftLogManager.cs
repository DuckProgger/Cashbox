using Cashbox.Exceptions;
using Cashbox.Model.Entities;
using Cashbox.Visu;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cashbox.Model.Managers
{
    public class ShiftLogManager : INotifyPropertyChanged
    {
        private int _salary;
        private DateTime endPeriod;
        private DateTime startPeriod;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Salary
        {
            get => _salary;
            set { _salary = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Shift> ShiftLog { get; private set; } = new();
        public ObservableCollection<string> Staff { get; private set; } = new();

        public Salary CalculateSalary()
        {
            const double Percent = 0.075;
            const int minDailySalary = 1000;

            double salary = 0;
            double dailySalary;

            List<Shift> list = ShiftLog.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                Shift shift = list[i];
                dailySalary = shift.Total * Percent;
                if (dailySalary < minDailySalary)
                    dailySalary = minDailySalary;
                salary += dailySalary;
            }
            return new Salary() { Money = (int)Math.Ceiling(salary), StartPeriod = startPeriod, EndPeriod = endPeriod };
        }

        public List<ShiftExcelItem> GetExcelShiftCollection()
        {
            List<ShiftExcelItem> collection = new();
            foreach (Shift item in ShiftLog)
                collection.Add(ShiftExcelItem.ConvertFromShift(item));
            return collection;
        }

        public Salary IssueSalary(string workerName)
        {
            Worker worker = DB.GetWorker(workerName);
            // проверка не выдана ли уже зарплата
            if (!IsValidSalaryCount(worker.Id, startPeriod, endPeriod))
                throw new SalaryCountException($"Cотрудник {workerName} уже получал ЗП" +
                                               $" за период с {Formatter.FormatDate(startPeriod)} " +
                                               $"по {Formatter.FormatDate(endPeriod)}");
            Salary salary = CalculateSalary();
            salary.WorkerId = worker.Id;
            return DB.Create(salary);
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void Update(DateTime start, DateTime end)
        {
            startPeriod = start;
            endPeriod = end;
            ShiftLog.Clear();
            Staff.Clear();
            foreach (Shift shift in DB.GetShiftLog(start, end))
            {
                ShiftLog.Add(shift);
                foreach (Worker worker in shift.Staff)
                    if (!Staff.Contains(worker.Name))
                        Staff.Add(worker.Name);
            }
        }

        private static bool IsValidSalaryCount(int workerId, DateTime start, DateTime end)
        {
            return DB.GetSalaries(workerId, start, end).Count == 0;
        }
    }
}