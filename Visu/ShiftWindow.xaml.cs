using Cashbox.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
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
    public partial class ShiftWindow : Window
    {
        private DB db = new();

        public ObservableCollection<Worker> Workers { get; private set; }
        public Shift Shift { get; set; }

        public ShiftWindow()
        {
            InitializeComponent();
            FillWorkers();
            DataContext = this;
        }

        private void SaveShift(object sender, RoutedEventArgs e)
        {
            db.SaveShift(Shift, Workers.ToList());
        }

        private void FillWorkers()
        {
            Shift = db.GetShift(DateTime.Now.Date);
            Workers = new(db.GetWorkers(Shift));
        }

        //((MainWindow)Owner).User.Permissions.IsAdmin
    }
}

