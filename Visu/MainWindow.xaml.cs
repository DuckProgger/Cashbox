using Cashbox.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

    public partial class MainWindow : Window
    {
        public MainWindow() { InitializeComponent(); }

        private void OpenShiftWindow(object sender, RoutedEventArgs e)
        {
            new ShiftWindow(DateTime.Today, Mode.NewVersion).Show();
        }

        private void OpenLogWindow(object sender, RoutedEventArgs e)
        {
            new LogWindow().Show();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            DB.RemoveSession(Global.Session.Id);
        }

        private void OpenStaffWindow(object sender, RoutedEventArgs e)
        {
            new StaffWindow().Show();
        }

        private void ChangeUser(object sender, RoutedEventArgs e)
        {
            new AuthorizationWindow().Show();
            Close();
        }
    }
}
