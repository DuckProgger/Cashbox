using Cashbox.Model;
using Cashbox.Model.Entities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cashbox.Visu
{
    public partial class AuthorizationWindow : Window, INotifyPropertyChanged
    {
        private string _selectedUser;

        public string SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(OkButtonVis));
            }
        }

        public MessageProvider ErrorMessage { get; } = new(true);
        public bool OkButtonVis => SelectedUser != null;

        public AuthorizationWindow()
        {
            InitializeComponent();
            DataContext = this;
            GetUsersAsync();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Session.InitSession(SelectedUser);
            new MainWindow().Show();
            Close();
        }

        private /*async*/ void GetUsersAsync()
        {
            try
            {
                Users.ItemsSource = /*await*/ DB.GetUserNamesAsync();
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage.Message = ex.Message;
            }
            catch (Exception)
            {
                ErrorMessage.Message = "Ошибка подключения к БД";
            }
        }

        private void UserChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedUser = ((ComboBox)sender).SelectedValue as string;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}