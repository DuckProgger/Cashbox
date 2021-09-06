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
    public partial class VersionHistoryWindow : Window
    {
        private readonly DateTime selectedDate;
        private int selectedVersion;

        public List<object> VersionHistory { get; set; }
        public Permissions Permissions { get; private set; }

        public VersionHistoryWindow(DateTime date)
        {
            InitializeComponent();
            DataContext = this;
            Permissions = Permissions.GetAccesses(AuthData.Session.UserId);
            selectedDate = date;
            VersionHistory = new(DB.GetShiftVersionHistory(selectedDate.Date));
            VersionHistoryView.ItemsSource = VersionHistory;
        }

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
