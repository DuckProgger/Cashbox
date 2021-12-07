using Cashbox.Exceptions;
using Cashbox.Model;
using Cashbox.Model.Entities;
using Cashbox.Model.Logging.Entities;
using Cashbox.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cashbox.Visu
{
    public partial class ShiftLogView : UserControl, INotifyPropertyChanged
    {
        #region privateProperties

        private const string issueQuestion = "Выдать сотруднику ЗП?";
        private const string removeQuestion = "Удалить выбранную смену?";
        private readonly IDialogService dialogService;
        private readonly IFileService<ShiftLogItem>[] fileServices;
        private string _dialogConfirmButtonText;
        private string _dialogQuestion;
        private DateTime _end;
        private bool _manualPeriodChecked;
        private string _selectedWorker;
        private DateTime memStart;
        private DateTime memEnd;
        private ObservableCollection<Shift> _shifts;
        private CollectionView shiftsView;
        private DateTime _start;
        private DateTime selectedShiftDate;

        #endregion privateProperties

        #region publicProperties

        public ObservableCollection<Shift> Shifts
        {
            get => _shifts ??= new();
            set
            {
                _shifts = value;
                shiftsView = (CollectionView)CollectionViewSource.GetDefaultView(_shifts);
                shiftsView.Filter = WorkerFilter;
                OnPropertyChanged(nameof(Staff));
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Staff => new(Worker.GetStaffInShifts(Shifts.ToList()));

        public string DialogConfirmButtonText
        {
            get => _dialogConfirmButtonText;
            set { _dialogConfirmButtonText = value; OnPropertyChanged(); }
        }

        public string DialogQuestion
        {
            get => _dialogQuestion;
            set { _dialogQuestion = value; OnPropertyChanged(); }
        }

        public DateTime End
        {
            get => _end;
            set { _end = value; OnPropertyChanged(); }
        }

        public MessageProvider ErrorMessage { get; } = new(true);
        public bool ExportButtonVis => Shifts?.Count > 0;

        public bool ManualPeriodChecked
        {
            get => _manualPeriodChecked;
            set
            { _manualPeriodChecked = value; OnPropertyChanged(); }
        }

        public bool SalaryButtonsVis => !string.IsNullOrEmpty(SelectedWorker);

        public string SelectedWorker
        {
            get => _selectedWorker;
            set
            {
                _selectedWorker = value;
                shiftsView?.Refresh();
                OnPropertyChanged(nameof(SalaryButtonsVis));
            }
        }

        public DateTime Start
        {
            get => _start;
            set { _start = value; OnPropertyChanged(); }
        }

        public MessageProvider StatusMessage { get; } = new(true);

        #endregion publicProperties

        public ShiftLogView()
        {
            InitializeComponent();
            DataContext = this;
            SetPrepaidPeriod(null, null);
            fileServices = new IFileService<ShiftLogItem>[] { new ExcelFileService<ShiftLogItem>() };
            dialogService = new DefaultDialog(fileServices);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void Button_GetLog(object sender, RoutedEventArgs e)
        {
            UpdateShifts();
            OnPropertyChanged(nameof(ExportButtonVis));
            memStart = Start;
            memEnd = End;
        }

        private void CalculateSalary_Click(object sender, RoutedEventArgs e)
        {
            Salary salary = Salary.CalculateSalary(SelectedWorker, memStart, memEnd);
            MessageBoxCustom.Show($"Сотрудник {SelectedWorker} получит {salary.Money} руб." +
                                 $" за период с {Formatter.FormatDate(memStart)} " +
                                 $"по {Formatter.FormatDate(memEnd)}",
                                 MessageType.Info, MessageButtons.Ok);
        }

        private void DialogOk_Click(object sender, RoutedEventArgs e)
        {
            switch (DialogQuestion)
            {
                case removeQuestion:
                    try
                    {
                        Shift.RemoveFromDB(selectedShiftDate);
                        UpdateShifts();
                    }
                    catch (Exception)
                    {
                        ErrorMessage.Message = "Не удалось удалить смену";
                    }
                    break;

                case issueQuestion:
                    try
                    {
                        Salary salary = Salary.AddSalary(SelectedWorker, memStart, memEnd);
                        MessageBoxCustom.Show($"Сотруднику {SelectedWorker} выдана ЗП в размере {salary.Money} руб." +
                                            $" за период с {Formatter.FormatDate(salary.StartPeriod)} " +
                                            $"по {Formatter.FormatDate(salary.EndPeriod)}",
                                            MessageType.Info, MessageButtons.Ok);
                    }
                    catch (SalaryCountException ex)
                    {
                        MessageBoxCustom.Show($"Операция отклонена: {ex.Message}", MessageType.Error, MessageButtons.Ok);
                    }
                    break;
            }
        }

        private void EditShift_Click(object sender, RoutedEventArgs e)
        {
            PopupWindow shiftWindow = new(new ShiftView(selectedShiftDate, new EditVersionMode()));
            shiftWindow.Show();
            shiftWindow.Closed += (s, e) => UpdateShifts();
        }

        private void UpdateShifts()
        {
            Shifts = new(Shift.GetShifts(Start, End));
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dialogService.SaveFileDialog())
                {
                    fileServices[dialogService.SelectedFormat - 1].SaveFile(dialogService.FilePath, Shift.GetExcelShiftCollection(memStart, memEnd));
                    StatusMessage.Message = $"Файл успешно экспортирован. Расположение {dialogService.FilePath}";
                }
            }
            catch (Exception)
            {
                ErrorMessage.Message = "Ошибка экспорта файла";
            }
        }

        private void IssueSalary_Click(object sender, RoutedEventArgs e)
        {
            DialogQuestion = issueQuestion;
            DialogConfirmButtonText = "Выдать";
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            selectedShiftDate = ((sender as ListViewItem).Content as Shift).CreatedAt.Date;
        }

        private void RemoveShift_Click(object sender, RoutedEventArgs e)
        {
            DialogQuestion = removeQuestion;
            DialogConfirmButtonText = "Удалить";
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

        private void VersionHistory_Click(object sender, RoutedEventArgs e)
        {
            new VersionHistoryWindow(selectedShiftDate).Show();
        }

        private void WatchShift_Click(object sender, RoutedEventArgs e)
        {
            new PopupWindow(new ShiftView(selectedShiftDate, new WatchOnlyMode())).Show();
        }

        private bool WorkerFilter(object item)
        {
            return string.IsNullOrEmpty(SelectedWorker) || (item as Shift).Staff.Find(w => w.Name == SelectedWorker) != null;
        }
    }
}