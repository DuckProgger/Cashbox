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
        private string _selectedWorkerName;
        private DateTime _start = Formatter.ReturnToFirstDay(DateTime.Today);
        private DateTime _end = Formatter.ReturnToEndOfMonth(DateTime.Today);
        private const string allWorkers = "(Все)";
        private Worker selectedWorker;

        public Permissions Permissions { get; private set; }
        public ObservableCollection<SalaryItem> SalaryLog { get; set; } = new();
        public ObservableCollection<string> Staff { get; set; } = new();
        public DateTime Start
        {
            get => _start;
            set
            {
                if (value > End)
                    _start = End;
                else
                    _start = value;
                OnPropertyChanged();
            }
        }
        public DateTime End
        {
            get => _end;
            set
            {
                if (value < Start)
                    _end = Start;
                else
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
        public string SelectedWorkerName
        {
            get => _selectedWorkerName;
            set
            {
                _selectedWorkerName = value;
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
            selectedWorker = DB.GetWorker(SelectedWorkerName);
            UpdateSalaryLog();
        }

        private void UpdateSalaryLog()
        {
            SalaryLog.Clear();
            var salaries = DB.GetSalaries(selectedWorker.Id, Start, End);
            foreach (var item in salaries)
            {
                SalaryItem salaryItem = new()
                {
                    Name = selectedWorker.Name,
                    Salary = item.Money,
                    Date = Formatter.FormatDatePeriod(item.StartPeriod, item.EndPeriod)
                };
                SalaryLog.Add(salaryItem);
            }
        }

        private void Button_GetSalaryLog(object sender, RoutedEventArgs e)
        {
            if (selectedWorker == null)
                MessageBoxCustom.Show("Не выбран работник", MessageType.Error, MessageButtons.Ok);
            else
                UpdateSalaryLog();
        }
    }
}
