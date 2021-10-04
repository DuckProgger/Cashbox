﻿using Cashbox.Exceptions;
using Cashbox.Model;
using Cashbox.Model.Entities;
using Cashbox.Model.Managers;
using Cashbox.Services;
using System;
using System.ComponentModel;
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
        private readonly IFileService<ShiftExcelItem>[] fileServices;
        private readonly CollectionView view;
        private string _dialogConfirmButtonText;
        private string _dialogQuestion;
        private DateTime _end;
        private bool _manualPeriodChecked;
        private string _selectedWorker;

        private DateTime _start;
        private DateTime selectedShiftDate;

        #endregion privateProperties

        #region publicProperties

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

        public MessageProvider ErrorMessage { get; } = new();
        public bool ExportButtonVis => ShiftLogManager.ShiftLog?.Count > 0;

        public bool ManualPeriodChecked
        {
            get => _manualPeriodChecked;
            set
            { _manualPeriodChecked = value; OnPropertyChanged(); }
        }

        public Permissions Permissions { get; private set; }
        public bool SalaryButtonsVis => !string.IsNullOrEmpty(SelectedWorker);

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

        public ShiftLogManager ShiftLogManager { get; private set; }

        public DateTime Start
        {
            get => _start;
            set { _start = value; OnPropertyChanged(); }
        }

        public MessageProvider StatusMessage { get; } = new();

        #endregion publicProperties

        public ShiftLogView()
        {
            InitializeComponent();
            DataContext = this;
            ShiftLogManager = new();
            Permissions = Permissions.GetAccesses(SessionManager.Session.UserId);
            SetPrepaidPeriod(null, null);
            fileServices = new IFileService<ShiftExcelItem>[] { new ExcelFileService<ShiftExcelItem>() };
            dialogService = new DefaultDialog(fileServices);
            view = (CollectionView)CollectionViewSource.GetDefaultView(ShiftLogManager.ShiftLog);
            view.Filter = WorkerFilter;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void Button_GetLog(object sender, RoutedEventArgs e)
        {
            ShiftLogManager.Update(Start, End);
            OnPropertyChanged(nameof(ExportButtonVis));
        }

        private void CalculateSalary_Click(object sender, RoutedEventArgs e)
        {
            Salary salary = ShiftLogManager.CalculateSalary();
            MessageBoxCustom.Show($"Сотрудник {SelectedWorker} получит {salary.Money} руб." +
                                 $" за период с {Formatter.FormatDate(salary.StartPeriod)} " +
                                 $"по {Formatter.FormatDate(salary.EndPeriod)}",
                                 MessageType.Info, MessageButtons.Ok);
        }

        private void DialogOk_Click(object sender, RoutedEventArgs e)
        {
            switch (DialogQuestion)
            {
                case removeQuestion:
                    try
                    {
                        ShiftManager.RemoveFromDB(selectedShiftDate);
                    }
                    catch (Exception)
                    {
                        ErrorMessage.Message = "Не удалось удалить смену";
                    }
                    ShiftLogManager.Update(Start, End);
                    break;

                case issueQuestion:
                    try
                    {
                        Salary salary = ShiftLogManager.IssueSalary(SelectedWorker);
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
            PopupWindow shiftWindow = new(new ShiftView(selectedShiftDate, Mode.EditVersion));
            shiftWindow.Show();
            shiftWindow.Closed += (s, e) => ShiftLogManager.Update(Start, End);
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dialogService.SaveFileDialog())
                {
                    fileServices[dialogService.SelectedFormat - 1].SaveFile(dialogService.FilePath, ShiftLogManager.GetExcelShiftCollection());
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
            new PopupWindow(new ShiftView(selectedShiftDate, Mode.WatchOnly)).Show();
        }

        private bool WorkerFilter(object item)
        {
            return string.IsNullOrEmpty(SelectedWorker) || (item as Shift).Staff.Find(w => w.Name == SelectedWorker) != null;
        }
    }
}