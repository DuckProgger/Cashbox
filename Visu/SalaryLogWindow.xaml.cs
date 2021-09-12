using Cashbox.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cashbox.Visu
{
    public partial class SalaryLogWindow : Window, INotifyPropertyChanged
    {
        private int _totalSalary;
        private string _selectedWorker;
        private DateTime _start = DateTime.Today;
        private DateTime _end = DateTime.Today;
        private const string allWorkers = "(Все)";

        public Permissions Permissions { get; private set; }
        public ObservableCollection<SalaryItem> SalaryLog { get; set; } = new();
        public ObservableCollection<string> Staff { get; set; } = new();
        public DateTime Start
        {
            get => _start;
            set
            {
                _start = value;
                OnPropertyChanged();
            }
        }
        public DateTime End
        {
            get => _end;
            set
            {
                _end = value;
                OnPropertyChanged();
            }
        }
        public int TotalSalary
        {
            get => _totalSalary;
            set
            {
                _totalSalary = value;
                OnPropertyChanged();
            }
        }
        public string SelectedWorker
        {
            get => _selectedWorker;
            set
            {
                _selectedWorker = value;
                OnPropertyChanged();
            }
        }

        public SalaryLogWindow()
        {
            InitializeComponent();
            DataContext = this;
            Staff.Add(allWorkers);
            foreach (var item in DB.GetStaff())
                Staff.Add(item.Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var worker = DB.GetWorker(SelectedWorker);
            UpdateSalaryLog(worker, Start, End);
        }

        private void UpdateSalaryLog(Worker worker, DateTime start, DateTime end)
        {
            SalaryLog.Clear();
            var salaries = DB.GetSalaries(worker.Id, start, end);
            foreach (var item in salaries)
            {
                SalaryItem salaryItem = new()
                {
                    Name = worker.Name,
                    Salary = item.Money,
                    Date = Formatter.FormatDatePeriod(item.StartPeriod, item.EndPeriod)
                };
                SalaryLog.Add(salaryItem);
            }
        }
    }
}
