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
        private DB db = new();
        private int _total;
        private int _difference;
        private string _differenceText;
        private readonly SolidColorBrush redBackground = new(Color.FromRgb(245, 94, 83));
        private readonly SolidColorBrush whiteBackground = new(Colors.White);

        public ObservableCollection<Worker> Workers { get; private set; }
        public Shift Shift { get; set; }
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

        public ShiftWindow()
        {
            InitializeComponent();
            db.CreateShift();
            Shift = db.GetShift(DateTime.Now.Date);
            UpdateValues();
            Workers = new(db.GetWorkers(Shift));
            DataContext = this;
        }

        private void SaveShift(object sender, RoutedEventArgs e)
        {
            db.SaveShift(Shift, Workers.ToList());
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
    }
}

