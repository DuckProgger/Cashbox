using Cashbox.Model.Entities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Cashbox.Visu
{
    public partial class VersionHistoryWindow : Window, INotifyPropertyChanged
    {
        private readonly DateTime selectedDate;
        private int selectedVersion;
        private ObservableCollection<Shift> _versionHistory;

        public ObservableCollection<Shift> VersionHistory
        {
            get => _versionHistory ??= new();
            set { _versionHistory = value; OnPropertyChanged(); }
        }

        public VersionHistoryWindow(DateTime date)
        {
            InitializeComponent();
            DataContext = this;
            selectedDate = date;
            VersionHistory = new(Shift.GetShifts(selectedDate.Date));
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            selectedVersion = ((Shift)(sender as ListViewItem).Content).Version;
        }

        private void WatchShift_Click(object sender, RoutedEventArgs e)
        {
            new PopupWindow(new ShiftView(selectedDate, new WatchOnlyMode(), selectedVersion)).Show();
        }

        private void EditShift_Click(object sender, RoutedEventArgs e)
        {
            new PopupWindow(new ShiftView(selectedDate, new WatchOnlyMode(), selectedVersion)).Show();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Shift.RemoveFromDB(selectedDate.Date, selectedVersion);
            VersionHistory = new(Shift.GetShifts(selectedDate.Date));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}