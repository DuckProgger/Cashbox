using Cashbox.Model;
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

namespace Cashbox.Visu
{

    public partial class ShiftWindow : Window, INotifyPropertyChanged
    {
        //private DB db = new();
        private int _total;
        private int _difference;
        private string _differenceText;
        private readonly SolidColorBrush redBackground = new(Color.FromRgb(245, 94, 83));
        private readonly SolidColorBrush whiteBackground = new(Colors.White);
        private readonly Mode viewMode;

        public ObservableCollection<WorkerItem> Staff { get; private set; }
        public Shift Shift { get; set; }
        public bool EnableEntries => viewMode != Mode.WatchOnly;
        public int Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged();
            }
        }
        public int Difference
        {
            get => _difference;
            set
            {
                _difference = value;
                OnPropertyChanged();
            }
        }
        public string DifferenceText
        {
            get => _differenceText;
            set
            {
                _differenceText = value;
                OnPropertyChanged();
            }
        }

        public ShiftWindow(DateTime date, Mode mode, int version = 0)
        {
            InitializeComponent();
            //DB.CreateShift();
            if (version == 0)
                Shift = DB.GetShift(date);
            else
                Shift = DB.GetShift(date, version);
            UpdateValues();
            Staff = new(DB.GetWorkerItems(Shift));
            DataContext = this;
            viewMode = mode;
        }


        //private static List<Worker> PrepareStaff(List<Worker> oldStaff, List<WorkerItem> staff)
        //{
        //    List<Worker> newStaff = new(oldStaff);
        //    foreach (var wi in newStaff)
        //    {
        //        // Найти работника по имени. 
        //        Worker worker = newStaff.Find(w => w.Name == wi.Name);
        //        // Добавить в смену отмеченного галочкой, если такого ещё нет.
        //        if (wi.Checked && worker == null)
        //            newStaff.Add(worker);
        //        // Убрать из смены работника без галочки, если он был.
        //        else if (!wi.Checked && worker != null)
        //            newStaff.Remove(worker);
        //    }
        //    return shift;
        //}


        private void SaveShift(object sender, RoutedEventArgs e)
        {
            switch (viewMode)
            {
                case Mode.WatchOnly:
                    break;
                case Mode.Edit:
                    //DB.EditShift(Shift, Staff.ToList());
                    break;
                case Mode.New:
                    //DB.SaveNewShift(Shift, Staff.ToList());
                    Shift = DB.UpdateShift(Shift);
                    break;
                default:
                    break;
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
            DB.CaclShift(Shift);
            UpdateValues();

            // Установить фон в поле Расхождения
            if (Difference > 0)
            {
                DifferenceText = "Недостача:";
                DifferenceBorder.Background = redBackground;
            }
            else if (Difference < 0)
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

        private void UpdateValues()
        {
            Total = Shift.Total;
            Difference = Shift.Difference;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string selectedName = ((WorkerItem)(sender as CheckBox).DataContext).Name;
            Worker worker = DB.GetWorker(selectedName);
            if (!WorkerExists(worker.Id))
                //Shift.Staff.Add(worker);
                Shift = DB.AddWorkerToShift(Shift.Id, worker.Id);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string selectedName = ((WorkerItem)(sender as CheckBox).DataContext).Name;
            Worker worker = DB.GetWorker(selectedName);
            if (WorkerExists(worker.Id))
                Shift = DB.RemoveWorkerFromShift(Shift.Id, worker.Id);

            //Shift.Staff.Remove(worker);
            //Shift.Staff.RemoveAt(Shift.Staff.FindIndex(w => w.Id == worker.Id));

        }

        private bool WorkerExists(int id) => Shift.Staff.Exists(w => w.Id == id);
    }
}

