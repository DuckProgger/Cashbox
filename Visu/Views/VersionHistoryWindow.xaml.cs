using Cashbox.Model;
using Cashbox.Model.Entities;
using Cashbox.Model.Managers;
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

        public ObservableCollection<Shift> VersionHistory { get; set; }
        public Permissions Permissions { get; private set; }

        public VersionHistoryWindow(DateTime date)
        {
            InitializeComponent();
            DataContext = this;
            Permissions = Permissions.GetAccesses(SessionManager.Session.UserId);
            selectedDate = date;
            VersionHistory = new(DB.GetShiftVersions(selectedDate.Date));
            VersionHistoryView.ItemsSource = VersionHistory;
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            selectedVersion = ((Shift)(sender as ListViewItem).Content).Version;
        }

        private void WatchShift_Click(object sender, RoutedEventArgs e)
        {
            new PopupWindow(new ShiftView(selectedDate, Mode.WatchOnly, selectedVersion)).Show();
        }

        private void EditShift_Click(object sender, RoutedEventArgs e)
        {
            new PopupWindow(new ShiftView(selectedDate, Mode.EditVersion, selectedVersion)).Show();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            DB.RemoveShift(selectedDate.Date, selectedVersion);
            UpdateVersionHistory();
        }

        private void UpdateVersionHistory()
        {
            VersionHistory.Clear();
            foreach (var item in DB.GetShiftVersions(selectedDate.Date))
                VersionHistory.Add(item);
        }
    }
}