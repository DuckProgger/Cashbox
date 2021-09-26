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

namespace Cashbox.Visu
{
    public partial class ShiftLogView : UserControl, INotifyPropertyChanged
    {
        #region privateProperties
        private DateTime selectedShiftDate;
        private int _salary;
        private bool _buttonsVis;
        private string _selectedWorker;
        private DateTime _start;
        private DateTime _end;
        private bool _manualPeriodChecked;
        private readonly IDialogService dialogService;
        private readonly IFileService<ShiftExcelItem>[] fileServices;
        private string _dialogQuestion;
        private const string removeQuestion = "Удалить выбранную смену?";
        private const string issueQuestion = "Выдать сотруднику ЗП?";
        private string _dialogConfirmButtonText;
        private readonly CollectionView view;
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
                view.Refresh();
                OnPropertyChanged(nameof(SalaryButtonsVis));
                //OnPropertyChanged(); // раскомментить, если нужно, чтобы при обновлении журнала автоматически выбирался сотрудник в ComboBox
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
        public string DialogQuestion
        {
            get => _dialogQuestion;
            set
            {
                _dialogQuestion = value;
                OnPropertyChanged();
            }
        }
        public string DialogConfirmButtonText
        {
            get => _dialogConfirmButtonText;
            set
            {
                _dialogConfirmButtonText = value;
                OnPropertyChanged();
            }
        }
        public MessageProvider ErrorMessage { get; } = new();
        public MessageProvider StatusMessage { get; } = new();
        public bool SalaryButtonsVis => !string.IsNullOrEmpty(SelectedWorker);
        public bool ExportButtonVis => Log?.Count > 0;
        #endregion

        public ShiftLogView()
        {
            InitializeComponent();
            DataContext = this;
            Permissions = Permissions.GetAccesses(Global.Session.UserId);
            SetPrepaidPeriod(null, null);
            fileServices = new IFileService<ShiftExcelItem>[] { new ExcelFileService<ShiftExcelItem>() };
            dialogService = new DefaultDialog(fileServices);
            view = (CollectionView)CollectionViewSource.GetDefaultView(Log);
            view.Filter = WorkerFilter;
        }

        private bool WorkerFilter(object item) => string.IsNullOrEmpty(SelectedWorker) || (item as Shift).Staff.Find(w => w.Name == SelectedWorker) != null;

        private void Button_GetLog(object sender, RoutedEventArgs e)
        {
            UpdateLog();
            UpdateStaffComboBox();
            //SelectedWorker = Staff.Count > 0 ? Staff[0] : null; // раскомментить, если нужно, чтобы при обновлении журнала автоматически выбирался сотрудник в ComboBox
        }

        private void UpdateLog()
        {
            Log.Clear();
            foreach (var item in DB.GetShiftLog(Start, End))
                Log.Add(item);
            OnPropertyChanged(nameof(ExportButtonVis));
        }

        private void UpdateStaffComboBox()
        {
            Staff.Clear();
            foreach (var item in Log)
            {
                foreach (var worker in item.Staff)
                    if (!Staff.Contains(worker.Name))
                        Staff.Add(worker.Name);
            }
        }

        private void VersionHistory_Click(object sender, RoutedEventArgs e) => new VersionHistoryWindow(selectedShiftDate).Show();

        private void WatchShift_Click(object sender, RoutedEventArgs e) => new PopupWindow(new ShiftView(selectedShiftDate, Mode.WatchOnly)).Show();

        private void EditShift_Click(object sender, RoutedEventArgs e) => new PopupWindow(new ShiftView(selectedShiftDate, Mode.EditVersion)).Show();

        private void DialogOk_Click(object sender, RoutedEventArgs e)
        {
            switch (DialogQuestion)
            {
                case removeQuestion:
                    DB.RemoveShift(selectedShiftDate);
                    UpdateLog();
                    break;
                case issueQuestion:
                    IssueSalary();
                    break;
                default:
                    break;
            }
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e) => selectedShiftDate = ((sender as ListViewItem).Content as Shift).CreatedAt.Date;

        private void CalculateSalary_Click(object sender, RoutedEventArgs e) => MessageBoxCustom.Show(Global.CalculateSalary(Log.ToList()).ToString(), MessageType.Info, MessageButtons.Ok);

        private void IssueSalary()
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
                    StatusMessage.Message = $"Файл успешно экспортирован. Расположение {dialogService.FilePath}";
                }
            }
            catch (Exception)
            {
                ErrorMessage.Message = "Ошибка экспорта файла";
            }
        }

        private void RemoveShift_Click(object sender, RoutedEventArgs e)
        {
            DialogQuestion = removeQuestion;
            DialogConfirmButtonText = "Удалить";
        }

        private void IssueSalary_Click(object sender, RoutedEventArgs e)
        {
            DialogQuestion = issueQuestion;
            DialogConfirmButtonText = "Выдать";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
