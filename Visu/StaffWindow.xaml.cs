using Cashbox.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class StaffWindow : Window, INotifyPropertyChanged
    {
        private string _newWorkerName;

        public ObservableCollection<WorkerItem> Staff { get; set; }
        public string NewWorkerName
        {
            get => _newWorkerName;
            set
            {
                _newWorkerName = value;
                OnPropertyChanged();
            }
        }
        public MessageProvider ErrorMessage { get; } = new();
        public string SearchEntry { get; set; }

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

        private void AddWorker_Click(object sender, RoutedEventArgs e)
        {
            if (IsEmptyName(NewWorkerName))
                ErrorMessage.Message = "Пустое имя.";
            else if (IsDublicate(NewWorkerName))
                ErrorMessage.Message = "Такой сотрудник уже есть в базе.";
            else
            {
                Worker newWorker = new() { Name = NewWorkerName, IsActive = true };
                DB.Create(newWorker);
                Staff.Add(new WorkerItem() { Name = newWorker.Name, Checked = newWorker.IsActive });
                NewWorkerName = string.Empty;
            }
        }

        private bool IsEmptyName(string name) => name?.Length == 0;

        private bool IsDublicate(string name) => DB.GetWorker(name) != null;

        private void NewWorkerNameField_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorMessage.Message = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void SearhField_TextChanged(object sender, TextChangedEventArgs e)
        {
            Staff = Staff.Where(w => w.Name == SearchEntry).ToList();
        }
    }
}
