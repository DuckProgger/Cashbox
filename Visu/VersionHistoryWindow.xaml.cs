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
        private DB db;
        private LogWindow owner;
        private DateTime selectedDate;

        public ObservableCollection<object> VersionHistory { get; set; } = new();

        public VersionHistoryWindow(DateTime date)
        {
            InitializeComponent();
            DataContext = this;
            selectedDate = date;
        }

        protected override void OnActivated(EventArgs e)
        {
            if (VersionHistory.Count == 0)
            {
                VersionHistoryView.ItemsSource = VersionHistory;
                owner = Owner as LogWindow;
                db = owner.DB;
                foreach (var item in db.GetShiftVersionHistory(selectedDate.Date))
                    VersionHistory.Add(item);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var obj = Util.Cast((sender as ListViewItem).Content, new { Time = DateTime.Now, Version = 0 });
            new ShiftWindow(selectedDate, Mode.Edit, obj.Version).Show();
        }
    }
}
