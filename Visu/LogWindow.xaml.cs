﻿using Cashbox.Model;
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
    public partial class LogWindow : Window
    {
        private DateTime selectedShiftDate;

        public Permissions Permissions { get; private set; }
        public ObservableCollection<object> Log { get; set; } = new();
        public DateTime Begin { get; set; } = DateTime.Today;
        public DateTime End { get; set; } = DateTime.Today;

        public LogWindow()
        {
            InitializeComponent();
            DataContext = this;
            LogView.ItemsSource = Log;
            Permissions = Permissions.GetAccesses(AuthData.Session.UserId);
        }

        private void Button_GetLog(object sender, RoutedEventArgs e) => UpdateLog();

        private void UpdateLog()
        {
            Log.Clear();
            foreach (var item in DB.GetLog(Begin, End))
                Log.Add(item);
        }

        private void VersionHistory_Click(object sender, RoutedEventArgs e)
        {
            VersionHistoryWindow versionHistoryWindow = new(selectedShiftDate) { Owner = this };
            versionHistoryWindow.Show();
        }

        private void WatchShift_Click(object sender, RoutedEventArgs e)
        {
            new ShiftWindow(selectedShiftDate, Mode.WatchOnly).Show();
        }

        private void EditShift_Click(object sender, RoutedEventArgs e)
        {
            new ShiftWindow(selectedShiftDate, Mode.EditVersion).Show();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            DB.RemoveShift(selectedShiftDate);
            UpdateLog();
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var obj = Util.Cast((sender as ListViewItem).Content, new { Date = DateTime.Now, Staff = "", Total = 0, Difference = 0 });
            selectedShiftDate = obj.Date;
        }
    }
}
