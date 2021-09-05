using Cashbox.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Cashbox.Visu
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window, INotifyPropertyChanged
    {
        //private DB db = new();
        private bool _okButtonVis;

        public string SelectedUser { get; set; }
        //public string EnteredPassword { get; set; }
        public bool OkButtonVis
        {
            get => _okButtonVis;
            set
            {
                _okButtonVis = value;
                OnPropertyChanged();
            }
        }

        public AuthorizationWindow()
        {
            InitializeComponent();
            GetUsersAsync();
            DataContext = this;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // Проверка правильности введённого пароля
            //if (db.CheckPassword(SelectedUser, EnteredPassword))
            //{
                new MainWindow(SelectedUser).Show();
                Close();
            //}
            //else
            //    ShowPopup("Неверный пароль");
        }

        private async void GetUsersAsync()
        {
            ShowPopup("Подключение к БД");
            //await Task.Run(() => DB.CreateDB());
            Users.ItemsSource = await DB.GetUserNamesAsync();
            MyPopup.IsPopupOpen = false;
        }

        private void UserChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedUser = ((ComboBox)sender).SelectedValue as string;
            CheckOkButtonVis();
        }

        //private void PasswordChanged(object sender, RoutedEventArgs e)
        //{
        //    EnteredPassword = ((PasswordBox)sender).Password;
        //    CheckOkButtonVis();
        //}

        private void ShowPopup(string text)
        {
            MyPopup.IsPopupOpen = true;
            PopupText.Text = text;
        }

        private void CheckOkButtonVis()
        {
            OkButtonVis = SelectedUser != null;
        }

        //protected override void OnClosed(EventArgs e)
        //{
        //    DB.Dispose();
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }       
    }
}
