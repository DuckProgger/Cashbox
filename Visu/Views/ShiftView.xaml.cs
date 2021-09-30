using Cashbox.Model;
using Cashbox.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MDDialogHost = MaterialDesignThemes.Wpf.DialogHost;


namespace Cashbox.Visu
{
    /// <summary>
    /// Логика взаимодействия для ShiftView.xaml
    /// </summary>
    public partial class ShiftView : UserControl, INotifyPropertyChanged
    {
        //private int _total;
        //private int _difference;
        private string _differenceText;
        private readonly SolidColorBrush redBackground = new(Color.FromRgb(245, 94, 83));
        private readonly SolidColorBrush whiteBackground = new(Colors.White);
        private readonly Mode viewMode;

        public ObservableCollection<WorkerViewItem> Staff { get; private set; }
        public Shift Shift { get; set; }
        public bool EnableEntries => viewMode != Mode.WatchOnly;
        //public int Total
        //{
        //    get => _total;
        //    set
        //    {
        //        _total = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public int Difference
        //{
        //    get => _difference;
        //    set
        //    {
        //        _difference = value;
        //        OnPropertyChanged();
        //    }
        //}
        public string DifferenceText
        {
            get => _differenceText;
            set
            {
                _differenceText = value;
                OnPropertyChanged();
            }
        }
        public MessageProvider ErrorMessage { get; } = new();
        public MessageProvider StatusMessage { get; } = new();

        public ShiftView(DateTime date, Mode mode, int version = 0)
        {
            InitializeComponent();
            if (version == 0)
            {
                Shift = DB.GetShift(date);
                if (Shift == null)
                {
                    Shift = Shift.Create(DB.GetUser(Manager.Session.UserId));
                    try
                    {
                        Shift.StartDay = DB.GetPrevShift().EndDay;
                    }
                    catch (InvalidOperationException)
                    {
                        Shift.StartDay = 0;
                    }
                }
            }
            else
                Shift = DB.GetShift(date, version);
            //UpdateValues();
            Staff = new(GetWorkerItems(Shift));
            DataContext = this;
            viewMode = mode;
        }

        public List<WorkerViewItem> GetWorkerItems(Shift shift)
        {
            List<WorkerViewItem> workers = new();
            foreach (Worker worker in DB.GetStaff())
            {
                if (worker.IsActive)
                {
                    WorkerViewItem workerItem = new() { Name = worker.Name };
                    // Поставить галочки работникам, которые были в смене.
                    if (shift.Staff != null && WorkerExists(worker.Id))
                        workerItem.Checked = true;
                    workers.Add(workerItem);
                }
            }
            return workers;
        }

        private async void SaveShift(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (viewMode)
                {
                    case Mode.WatchOnly:
                        break;
                    case Mode.EditVersion:
                        DB.UpdateShift(Shift);
                        break;
                    case Mode.NewVersion:
                        DB.CreateShift(Shift);
                        break;
                    default:
                        break;
                }
                StatusMessage.Message = "Смена успешно сохранена.";
                await Task.Delay(3000);
                StatusMessage.Message = string.Empty;
            }
            catch (Exception)
            {
                ErrorMessage.Message = "Не удалось сохранить смену.";
            }
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

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text?.Length == 0)
                textBox.Text = "0";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Отформатировать введенный текст
            TextBox textBox = sender as TextBox;
            textBox.Text = Formatter.Format(textBox.Text, Formatter.Type.MoneyEnter);
            // Переместить курсор в конец
            textBox.SelectionStart = textBox.Text.Length;

            // Рассчитать новые значения
            //Manager.CaclShift(Shift);
            //UpdateValues();

            // Установить фон в поле Расхождения
            if (Shift.Difference > 0)
            {
                DifferenceText = "Недостача:";
                DifferenceBorder.Background = redBackground;
            }
            else if (Shift.Difference < 0)
            {
                DifferenceText = "Излишек:";
                DifferenceBorder.Background = redBackground;
            }
            else
            {
                DifferenceText = "Расхождение:";
                DifferenceBorder.Background = whiteBackground;
            }
        }

        //private void UpdateValues()
        //{
        //    Total = Shift.Total;
        //    Difference = Shift.Difference;
        //}

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Message = string.Empty;
            string selectedName = ((WorkerViewItem)(sender as CheckBox).DataContext).Name;
            Worker worker = DB.GetWorker(selectedName);
            if (!WorkerExists(worker.Id))
                Shift.Staff.Add(worker);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string selectedName = ((WorkerViewItem)(sender as CheckBox).DataContext).Name;
            Worker worker = DB.GetWorker(selectedName);
            if (WorkerExists(worker.Id))
                Shift.Staff.RemoveAt(Shift.Staff.FindIndex(w => w.Id == worker.Id));
        }

        private bool WorkerExists(int id) => Shift.Staff.Exists(w => w.Id == id);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidStaffList(Shift))
                ErrorMessage.Message = "В смене нет работников";
            else
                MDDialogHost.OpenDialogCommand.Execute(null, null);
        }

        private bool IsValidStaffList(Shift shift) => shift.Staff.Count > 0;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

