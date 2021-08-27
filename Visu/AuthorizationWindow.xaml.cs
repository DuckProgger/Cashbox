using Cashbox.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class AuthorizationWindow : Window
    {
        private DB db = new();

        public string SelectedUser { get; set; }
        public string EnteredPassword { get; set; }

        public AuthorizationWindow()
        {
            InitializeComponent();
            GetUsersAsync();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // Проверка правильности введённого пароля
            if (db.CheckPassword(SelectedUser, EnteredPassword))
            {
                new MainWindow().Show();
                Close();
            }
            else
                ShowPopup("Неверный пароль");
        }             

        private async void GetUsersAsync()
        {
            ShowPopup("Подключение к БД");
            db = await Task.Run(() => DB.CreateDB());
            Users.ItemsSource = await db.GetUserNamesAsync();
            MyPopup.IsPopupOpen = false;
        }

        private void UserChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedUser = ((ComboBox)sender).SelectedValue as string;
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            EnteredPassword = ((PasswordBox)sender).Password;
        }

        private void ShowPopup(string text)
        {
            MyPopup.IsPopupOpen = true;
            PopupText.Text = text;
        }

        protected override void OnClosed(EventArgs e)
        {
            db.Dispose();
        }
    }
}
