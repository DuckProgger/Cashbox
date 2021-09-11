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
    public partial class AuthorizationWindow : Window, INotifyPropertyChanged
    {
        private bool _okButtonVis;

        public string SelectedUser { get; set; }
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
            Global.InitSession(SelectedUser);
            new MainWindow().Show();
            Close();
        }

        private async void GetUsersAsync()
        {
            Users.ItemsSource = await DB.GetUserNamesAsync();  
        }

        private void UserChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedUser = ((ComboBox)sender).SelectedValue as string;
            CheckOkButtonVis();
        }

        private void CheckOkButtonVis()
        {
            OkButtonVis = SelectedUser != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
