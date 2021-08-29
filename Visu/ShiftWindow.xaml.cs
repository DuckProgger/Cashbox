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
        private decimal _total;
        private decimal _difference;
        private string _differenceText;
        private Color diffBackgroung = Color.FromRgb(245, 94, 83);

        public ObservableCollection<Worker> Workers { get; private set; }
        public Shift Shift { get; set; }
        public decimal Total
        {
            get => _total;
            set
            {
                _total = value;
                OnPropertyChanged();
            }
        }
        public decimal Difference
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
            Shift = db.GetShift(DateTime.Now.Date);
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

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == decimal.Zero.ToString())
                textBox.Text = string.Empty;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text?.Length == 0)
                textBox.Text = decimal.Zero.ToString();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalcTotalAndDifference();
            if (Difference > 0)
            {
                DifferenceText = "Недостача:";
                DifferenceBorder.Background = new SolidColorBrush(diffBackgroung);
            }
            else if (Difference < 0)
            {
                DifferenceText = "Излишек:";
                DifferenceBorder.Background = new SolidColorBrush(diffBackgroung);
            }
            else
            {
                DifferenceText = "Расхождение:";
                DifferenceBorder.Background = new SolidColorBrush(Colors.White);
            }
        }

        private void CalcTotalAndDifference()
        {
            Total = Shift.Cash + Shift.Terminal;
            Difference = Shift.Cash - Shift.Expenses + Shift.StartDay - Shift.EndDay - Shift.HandedOver;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

