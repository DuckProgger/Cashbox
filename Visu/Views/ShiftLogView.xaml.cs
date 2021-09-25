using Cashbox.Model;
using Cashbox.Services;
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
    public partial class ShiftLogView : UserControl, INotifyPropertyChanged
    {
        #region privateProperties
        private DateTime selectedShiftDate;
        private int _salary;
        private bool _salaryButtonVis;
        private string _selectedWorker;
        private DateTime _start;
        private DateTime _end;
        private bool _manualPeriodChecked = false;
        private readonly IDialogService dialogService;
        private readonly IFileService<ShiftExcelItem>[] fileServices;
        #endregion

        #region publicProperties
        public Permissions Permissions { get; private set; }
        public ObservableCollection<Shift> Log { get; set; } = new();
        public ObservableCollection<string> Staff { get; set; } = new();
        public DateTime Start
        {
            get => _start;
            set
            {
                _start = value; OnPropertyChanged();
            }
        }
        public DateTime End
        {
            get => _end;
            set
            {
                _end = value; OnPropertyChanged();
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
        public bool ManualPeriodChecked
        {
            get => _manualPeriodChecked;
            set
            {
                _manualPeriodChecked = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public ShiftLogView()
        {
            InitializeComponent();
            DataContext = this;
            Permissions = Permissions.GetAccesses(Global.Session.UserId);
            SetPrepaidPeriod(null, null);
            fileServices = new IFileService<ShiftExcelItem>[] { new ExcelFileService<ShiftExcelItem>() };
            dialogService = new DefaultDialog(fileServices);
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
            SelectedWorker = Staff.Count > 0 ? Staff[0] : null;
        }

        private void VersionHistory_Click(object sender, RoutedEventArgs e)
        {
            new VersionHistoryWindow(selectedShiftDate).Show();
        }

        private void WatchShift_Click(object sender, RoutedEventArgs e)
        {
            new PopupWindow(new ShiftView(selectedShiftDate, Mode.WatchOnly)).Show();
        }

        private void EditShift_Click(object sender, RoutedEventArgs e)
        {
            new PopupWindow(new ShiftView(selectedShiftDate, Mode.EditVersion)).Show();
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

        private void SetPrepaidPeriod(object sender, RoutedEventArgs e)
        {
            Start = DateTime.Today;
            End = Start;
            Start = Formatter.ReturnToFirstDay(Start);
            End = Formatter.ReturnToMiddleOfMonth(End);
        }

        private void SetSalaryPeriod(object sender, RoutedEventArgs e)
        {
            Start = DateTime.Today;
            End = Start;
            Start = Formatter.ReturnToMiddleOfMonth(Start);
            Start = Start.AddDays(1);
            End = Formatter.ReturnToEndOfMonth(End);
        }

        private static bool IsValidSalaryCount(int workerId, DateTime start, DateTime end) => DB.GetSalaries(workerId, start, end).Count == 0;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dialogService.SaveFileDialog())
                {
                    List<ShiftExcelItem> collection = new();
                    foreach (Shift item in Log)
                        collection.Add(ShiftExcelItem.ConvertFromShift(item));
                    fileServices[dialogService.SelectedFormat - 1].SaveFile(dialogService.FilePath, collection);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
