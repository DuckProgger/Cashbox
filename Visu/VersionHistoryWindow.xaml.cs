using Cashbox.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Cashbox.Visu
{
    /// <summary>
    /// Логика взаимодействия для VersionHistoryWindow.xaml
    /// </summary>
    public partial class VersionHistoryWindow : Window
    {
        //private LogWindow owner;
        private readonly DateTime selectedDate;
        private int selectedVersion;

        public ObservableCollection<object> VersionHistory { get; set; } = new();
        public bool IsAdmin { get; set; } = false;

        public VersionHistoryWindow(DateTime date)
        {
            InitializeComponent();
            DataContext = this;
            VersionHistoryView.ItemsSource = VersionHistory;
            selectedDate = date;
            foreach (var item in DB.GetShiftVersionHistory(selectedDate.Date))
                VersionHistory.Add(item);
            //foreach (Window window in App.Current.Windows)
            //    if (window is LogWindow logWindow)                    
            //        IsAdmin = logWindow.IsAdmin;
            IsAdmin = (App.Current.MainWindow as MainWindow).User.Permissions.IsAdmin;
        }

        //protected override void OnActivated(EventArgs e)
        //{
        //    if (VersionHistory.Count == 0)
        //    {
        //        owner = Owner as LogWindow;
        //        foreach (var item in DB.GetShiftVersionHistory(selectedDate.Date))
        //            VersionHistory.Add(item);
        //        IsAdmin = owner.IsAdmin;
        //    }
        //}

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var obj = Util.Cast((sender as ListViewItem).Content, new { Time = DateTime.Now, Version = 0 });
            selectedVersion = obj.Version;
        }

        private void WatchShift_Click(object sender, RoutedEventArgs e)
        {
            new ShiftWindow(selectedDate, Mode.WatchOnly, selectedVersion).Show();
        }

        private void EditShift_Click(object sender, RoutedEventArgs e)
        {
            new ShiftWindow(selectedDate, Mode.EditVersion, selectedVersion).Show();
        }
    }
}
