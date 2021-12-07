using Cashbox.Model.Entities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cashbox.Visu
{
    //public enum Mode : int { WatchOnly, EditVersion, NewVersion }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ShiftLogView logView = new();
        private readonly SalaryLogView salaryLogView = new();
        private readonly StaffView staffView = new();
        private object _currentView;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public bool IsAdmin => Session.GetPermissions().IsAdmin;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Session.RemoveCurrentSession();
        }

        private void ChangeUser(object sender, RoutedEventArgs e)
        {
            new AuthorizationWindow().Show();
            Close();
        }

        private void Expander_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Expander).IsExpanded = true;
        }

        private void Expander_MouseLeave(object sender, MouseEventArgs e)
        {
            if (CurrentView != null)
                (sender as Expander).IsExpanded = false;
        }

        private void OpenLogView(object sender, RoutedEventArgs e)
        {
            CurrentView = logView;
        }

        private void OpenSalaryLogView(object sender, RoutedEventArgs e)
        {
            CurrentView = salaryLogView;
        }

        private void OpenShiftView(object sender, RoutedEventArgs e)
        {
            CurrentView = new ShiftView(DateTime.Today, new NewVersionMode());
        }

        private void OpenStaffView(object sender, RoutedEventArgs e)
        {
            CurrentView = staffView;
        }
    }
}