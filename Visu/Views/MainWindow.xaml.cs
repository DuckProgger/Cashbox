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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cashbox.Visu
{
    public enum Mode : int { WatchOnly, EditVersion, NewVersion }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private object _currentView;
        private readonly ShiftLogView logView = new();
        private readonly StaffView staffView = new();
        private readonly SalaryLogView salaryLogView = new();


        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OpenShiftView(object sender, RoutedEventArgs e) => CurrentView = new ShiftView(DateTime.Today, Mode.NewVersion);

        private void OpenLogView(object sender, RoutedEventArgs e) => CurrentView = logView;

        protected override void OnClosing(CancelEventArgs e) => DB.RemoveSession(Global.Session.Id);

        private void OpenStaffView(object sender, RoutedEventArgs e) => CurrentView = staffView;

        private void OpenSalaryLogView(object sender, RoutedEventArgs e) => CurrentView = salaryLogView;

        private void ChangeUser(object sender, RoutedEventArgs e)
        {
            new AuthorizationWindow().Show();
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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
    }
}
