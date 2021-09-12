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
    public partial class LogWindow : Window, INotifyPropertyChanged
    {
        private DateTime selectedShiftDate;
        private int _salary;
        private bool _salaryButtonVis;
        private string _selectedWorker;
        private DateTime _start;
        private DateTime _end;

        public Permissions Permissions { get; private set; }
        public ObservableCollection<Shift> Log { get; set; } = new();
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
        public bool SalaryButtonVis
        {
            get => _salaryButtonVis;
            set
            {
                _salaryButtonVis = value;
                OnPropertyChanged();
            }
        }
        public int Salary
        {
            get => _salary;
            set
            {
                _salary = value;
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


        public LogWindow()
        {
            InitializeComponent();
            DataContext = this;
            //LogView.ItemsSource = Log;
            Permissions = Permissions.GetAccesses(Global.Session.UserId);
        }

        private void Button_GetLog(object sender, RoutedEventArgs e) => UpdateLog();

        private void UpdateLog()
        {
            Log.Clear();
            Staff.Clear();
            foreach (var item in DB.GetShiftLog(Start, End))
            {
                Log.Add(item);
                foreach (var worker in item.Staff)
                    if (!Staff.Contains(worker.Name))
                        Staff.Add(worker.Name);
            }
            SelectedWorker = Staff[0] ?? null;
        }

        private void VersionHistory_Click(object sender, RoutedEventArgs e)
        {
            VersionHistoryWindow versionHistoryWindow = new(selectedShiftDate) { Owner = this };
            versionHistoryWindow.Show();
        }

        private void WatchShift_Click(object sender, RoutedEventArgs e)
        {
            new ShiftWindow(selectedShiftDate, Mode.WatchOnly).Show();
        }

        private void EditShift_Click(object sender, RoutedEventArgs e)
        {
            new ShiftWindow(selectedShiftDate, Mode.EditVersion).Show();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            DB.RemoveShift(selectedShiftDate);
            UpdateLog();
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            selectedShiftDate = ((Shift)(sender as ListViewItem).Content).CreatedAt.Date; ;
        }

        private void CalculateSalary_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxCustom.Show(Global.CalculateSalary(Log.ToList()).ToString(), MessageType.Info, MessageButtons.Ok);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SalaryButtonVis = sender != null;
            foreach (var item in Log.ToList())
                if (item.Staff.Find(w => w.Name == SelectedWorker) == null)
                    Log.Remove(item);
        }

        private void IssueSalary_Click(object sender, RoutedEventArgs e)
        {
            var worker = DB.GetWorker(SelectedWorker);
            // проверка не выдана ли уже зарплата
            if (!IsValidSalaryCount(worker.Id, Start, End))
                MessageBoxCustom.Show("Этот сотрудник уже получал ЗП за выбранный период", MessageType.Error, MessageButtons.Ok);
            else
            {
                int money = Global.CalculateSalary(Log.ToList());
                DB.Create(new Salary() { WorkerId = worker.Id, Money = money, StartPeriod = Start, EndPeriod = End });
                MessageBoxCustom.Show($"Сотруднику {worker.Name} выдана ЗП в размере {money} руб." +
                    $" за период с {Formatter.FormatDate(Start.Date)} по {Formatter.FormatDate(End.Date)}",
                    MessageType.Info, MessageButtons.Ok);
            }
        }

        private static bool IsValidSalaryCount(int workerId, DateTime start, DateTime end) => DB.GetSalaries(workerId, start, end).Count == 0;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void PrepaidPeriod_Checked(object sender, RoutedEventArgs e)
        {
            Start = DateTime.Today;
            End = Start;
            Start = ReturnToFirstDay(Start);
            End = ReturnToMiddleOfMonth(End);
        }

        private void SalaryPeriod_Checked(object sender, RoutedEventArgs e)
        {
            Start = DateTime.Today;
            End = Start;
            Start = ReturnToMiddleOfMonth(Start);
            End = ReturnToEndOfMonth(End);
        }

        private void ManuallyPeriod_Checked(object sender, RoutedEventArgs e)
        {

        }

        private DateTime ReturnToFirstDay(DateTime date) => date.AddDays(-date.Day + 1);

        private DateTime ReturnToMiddleOfMonth(DateTime date)
        {
            date = ReturnToFirstDay(date);
            date = date.AddDays(14);
            return date;
        }

        private DateTime ReturnToEndOfMonth(DateTime date)
        {
            date = ReturnToFirstDay(date);
            date = date.AddMonths(1);
            date = date.AddDays(-1);
            return date;
        }

        private void ShowSalaryLog(object sender, RoutedEventArgs e)
        {
            new SalaryLogWindow().Show();
        }
    }
}
