using Cashbox.Model.Entities;
using Cashbox.Model.Managers;
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
            VersionHistory = new(ShiftManager.GetShifts(selectedDate.Date));
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
            ShiftManager.RemoveFromDB(selectedDate.Date, selectedVersion);
            VersionHistory = new(ShiftManager.GetShifts(selectedDate.Date));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}