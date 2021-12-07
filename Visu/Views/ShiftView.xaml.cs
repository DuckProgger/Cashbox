using Cashbox.Model.Entities;
using Cashbox.Visu.ViewEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MDDialogHost = MaterialDesignThemes.Wpf.DialogHost;

namespace Cashbox.Visu
{
    public partial class ShiftView : UserControl, INotifyPropertyChanged
    {
        #region privateFields

        private readonly SolidColorBrush redBackground = new(Color.FromRgb(245, 94, 83));
        private readonly SolidColorBrush whiteBackground = new(Colors.White);
        private Shift _shift;
        private readonly int memStartDay;
        private bool startDayChanged;

        #endregion privateFields

        public ShiftView(DateTime date, IOpenMode mode, int version = 0)
        {
            InitializeComponent();
            Shift = Shift.GetShift(date, version);
            Staff = Shift.GetWorkerViewItems(Shift);
            DataContext = this;
            ViewMode = mode;
            memStartDay = Shift.StartDay;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region publicProperties

        public List<WorkerViewItem> Staff { get; set; }
        public string DifferenceText
        {
            get
            {
                if (Shift.Difference > 0)
                {
                    DifferenceBorder.Background = redBackground;
                    return "Недостача:";
                }
                else if (Shift.Difference < 0)
                {
                    DifferenceBorder.Background = redBackground;
                    return "Излишек:";
                }
                else
                {
                    DifferenceBorder.Background = whiteBackground;
                    return "Расхождение:";
                }
            }
        }
        public Shift Shift { get => _shift; set => _shift = value; }
        public IOpenMode ViewMode { get; private set; }
        public MessageProvider ErrorMessage { get; } = new(true);
        public MessageProvider WarningMessage { get; } = new();
        public MessageProvider StatusMessage { get; } = new(true);

        #endregion publicProperties

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Shift.Validate(Shift))
            {
                ErrorMessage.Message = "В смене нет работников";
            }
            else
            {
                WarningMessage.Message = startDayChanged
                    ? "\nВнимание! Поле \"Начало дня\" не совпадает с полем \"Конец дня\" прошлой смены"
                    : string.Empty;
                MDDialogHost.OpenDialogCommand.Execute(null, null);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Message = string.Empty;
            string selectedName = ((WorkerViewItem)(sender as CheckBox).DataContext).Name;
            Shift.AddWorker(selectedName);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string selectedName = ((WorkerViewItem)(sender as CheckBox).DataContext).Name;
            Shift.RemoveWorker(selectedName);
        }

        private void Comment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                textBox.Text += Environment.NewLine;
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
            }
        }

        private void SaveShift(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewMode.SaveShift(Shift);
                StatusMessage.Message = "Смена успешно сохранена.";
            }
            catch (Exception)
            {
                ErrorMessage.Message = "Не удалось сохранить смену.";
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnPropertyChanged(nameof(DifferenceText));
            startDayChanged = memStartDay != Shift.StartDay;
        }
    }

    public interface IOpenMode
    {
        public void SaveShift(Shift shift);
        public bool EnableEntries { get; }
        public bool StartDayFieldReadOnly { get; }
    }

    public class WatchOnlyMode : IOpenMode
    {
        public bool EnableEntries => false;
        public bool StartDayFieldReadOnly => true;

        public void SaveShift(Shift shift)
        {
            throw new NotImplementedException();
        }
    }

    public class EditVersionMode : IOpenMode
    {
        public bool EnableEntries => true;
        public bool StartDayFieldReadOnly => false;

        public void SaveShift(Shift shift)
        {
            Shift.UpdateDB(shift);
        }
    }

    public class NewVersionMode : IOpenMode
    {
        public bool EnableEntries => true;
        public bool StartDayFieldReadOnly => true;

        public void SaveShift(Shift shift)
        {
            Shift.AddToDB(shift);
        }
    }
}