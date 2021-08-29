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


            //List<User> users = new();
            //users.Add(db.GetUser("Администратор"));
          
            //Shift shift1 = new Shift()
            //{
            //    Date = DateTime.Now.Date,
            //    Users = users
            //};
            //db.AddShift(shift1);
        }

        private void OpenShiftWindow(object sender, RoutedEventArgs e)
        {
            ShiftWindow shiftWindow = new ShiftWindow();
            shiftWindow.Owner = this;
            shiftWindow.Show();
        }
    }
}
