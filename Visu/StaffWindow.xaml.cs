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
    /// Логика взаимодействия для WorkersWindow.xaml
    /// </summary>
    public partial class StaffWindow : Window
    {
        public ObservableCollection<WorkerItem> Staff { get; set; }


        public StaffWindow()
        {
            InitializeComponent();
            DataContext = this;
            Staff = new(GetWorkerItems());
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string selectedName = ((WorkerItem)(sender as CheckBox).DataContext).Name;
            Worker worker = DB.GetWorker(selectedName);
            worker.IsActive = true;
            DB.UpdateWorker(worker);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string selectedName = ((WorkerItem)(sender as CheckBox).DataContext).Name;
            Worker worker = DB.GetWorker(selectedName);
            worker.IsActive = false;
            DB.UpdateWorker(worker);
        }

        public List<WorkerItem> GetWorkerItems()
        {
            List<WorkerItem> workers = new();
            foreach (Worker worker in DB.GetStaff())
            {
                WorkerItem workerItem = new() { Name = worker.Name };
                // Поставить галочки действующим работникам.
                if (worker.IsActive)
                    workerItem.Checked = true;
                workers.Add(workerItem);
            }
            return workers;
        }
    }
}
