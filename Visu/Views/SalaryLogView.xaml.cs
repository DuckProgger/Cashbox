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
    /// <summary>
    /// Логика взаимодействия для SalaryLogView.xaml
    /// </summary>
    public partial class SalaryLogView : UserControl, INotifyPropertyChanged
    {
        #region privateProperties
        private int _totalSalary;
        private string _selectedWorkerName;
        private DateTime _start = Formatter.ReturnToFirstDay(DateTime.Today);
        private DateTime _end = Formatter.ReturnToEndOfMonth(DateTime.Today);
        private const string allWorkers = "(Все)";
        private Worker selectedWorker;
        private bool _combinePerMonth;
        #endregion

        #region publicProperties
        public Permissions Permissions { get; private set; }
        public ObservableCollection<SalaryViewItem> SalaryLog { get; set; } = new();
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
        public bool CombinePerMonth
        {
            get => _combinePerMonth;
            set
            {
                _combinePerMonth = value;
                if (IsValidWorkerSelected())
                    UpdateSalaryLog();
                OnPropertyChanged();
            }
        }
        public MessageProvider ErrorMessage { get; } = new();
        #endregion

        public SalaryLogView()
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
            ErrorMessage.Message = string.Empty;
            selectedWorker = DB.GetWorker(SelectedWorkerName);
            UpdateSalaryLog();
        }

        private void UpdateSalaryLog()
        {
            SalaryLog.Clear();

            // Сформировать список зарплат в зависимости от того выбран ли конкретный работник
            // или список нужно получить для всех за выбранный период
            List<Salary> salariesPerPeriod = SelectedWorkerName == allWorkers ?
                 DB.GetSalaries(Start, End) : DB.GetSalaries(selectedWorker.Id, Start, End);

            // Заполнение таблицы
            // *не объединять в месяц*
            if (!CombinePerMonth)
            {
                // Так как объединять в месяц не нужно, то просто перебираем список зарплат
                // за выбранный период и выводим его на экран
                foreach (var salary in salariesPerPeriod)
                {
                    SalaryLog.Add(new()
                    {
                        Name = DB.GetWorker(salary.WorkerId).Name,
                        Salary = salary.Money,
                        Date = Formatter.FormatDatePeriod(salary.StartPeriod, salary.EndPeriod)
                    });
                }
            }
            // *объединять в месяц*
            else
            {
                // Создать словарь, где ключом является Id работника,
                // а значением является список его смен за выбранный период 
                Dictionary<int, List<Salary>> workersSalariesDict = new();

                // Сгруппировать список смен за выбранный период по Id работников
                var workersSalaries = from s in salariesPerPeriod
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
                        SalaryLog.Add(salaryItem);
                }
            }
        }

        private void Button_GetSalaryLog(object sender, RoutedEventArgs e)
        {           
            if (!IsValidWorkerSelected())
                ErrorMessage.Message = "Не выбран работник";
            else
                UpdateSalaryLog();
        }

        private bool IsValidWorkerSelected() => SelectedWorkerName?.Length > 0;
    }
}
