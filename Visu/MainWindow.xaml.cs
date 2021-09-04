using Cashbox.Model;
using System;
using System.Collections.Generic;
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
    public enum Mode : int { ReadOnly, Edit, New }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User User { get; set; }
        private DB db = new();

        public MainWindow(string user)
        {
            InitializeComponent();
            User = db.GetUser(user);
        }

        private void OpenShiftWindow(object sender, RoutedEventArgs e)
        {            
            ShiftWindow shiftWindow = new(DateTime.Today, Mode.New);
            shiftWindow.Owner = this;
            shiftWindow.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogWindow lodWindow = new();
            lodWindow.Owner = this;
            lodWindow.Show();
        }
    }
}
