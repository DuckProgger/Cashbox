using Cashbox.Model;
using Cashbox.Model.Managers;
using Cashbox.Visu.ViewEntities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MDDialogHost = MaterialDesignThemes.Wpf.DialogHost;

namespace Cashbox.Visu
{
    public partial class ShiftView : UserControl, INotifyPropertyChanged
    {
        private readonly SolidColorBrush redBackground = new(Color.FromRgb(245, 94, 83));

        private readonly Mode viewMode;

        private readonly SolidColorBrush whiteBackground = new(Colors.White);

        private string _differenceText;

        public ShiftView(DateTime date, Mode mode, int version = 0)
        {
            InitializeComponent();
            ShiftManager = new(date, version);
            DataContext = this;
            viewMode = mode;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string DifferenceText
        {
            get => _differenceText;
            set
            {
                _differenceText = value;
                OnPropertyChanged();
            }
        }

        public bool EnableEntries => viewMode != Mode.WatchOnly;
        public MessageProvider ErrorMessage { get; } = new();
        public ShiftManager ShiftManager { get; private set; }
        public MessageProvider StatusMessage { get; } = new();

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!ShiftManager.IsValidStaffList())
                ErrorMessage.Message = "В смене нет работников";
            else
                MDDialogHost.OpenDialogCommand.Execute(null, null);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Message = string.Empty;
            string selectedName = ((WorkerViewItem)(sender as CheckBox).DataContext).Name;
            ShiftManager.AddWorker(selectedName);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string selectedName = ((WorkerViewItem)(sender as CheckBox).DataContext).Name;
            ShiftManager.RemoveWorker(selectedName);
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

        private async void SaveShift(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (viewMode)
                {
                    case Mode.WatchOnly:
                        break;

                    case Mode.EditVersion:
                        ShiftManager.UpdateDB();
                        break;

                    case Mode.NewVersion:
                        ShiftManager.AddToDB();
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

            // Установить фон в поле Расхождения
            if (ShiftManager.Shift.Difference > 0)
            {
                DifferenceText = "Недостача:";
                DifferenceBorder.Background = redBackground;
            }
            else if (ShiftManager.Shift.Difference < 0)
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
    }
}