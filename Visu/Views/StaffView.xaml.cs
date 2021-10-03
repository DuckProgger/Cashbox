using Cashbox.Model;
using Cashbox.Model.Entities;
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
using Cashbox.Visu.ViewEntities;

namespace Cashbox.Visu
{
    /// <summary>
    /// Логика взаимодействия для StaffView.xaml
    /// </summary>
    public partial class StaffView : UserControl, INotifyPropertyChanged
    {
        private string _newWorkerName;
        private string _searchEntry;
        private readonly CollectionView view;

        public ObservableCollection<WorkerViewItem> Staff { get; set; }

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

        public string SearchEntry
        {
            get => _searchEntry;
            set
            {
                _searchEntry = value;
                view.Refresh();
            }
        }

        public StaffView()
        {
            InitializeComponent();
            DataContext = this;
            Staff = new(GetWorkerItems());
            view = (CollectionView)CollectionViewSource.GetDefaultView(Staff);
            view.Filter = WorkersFilter;
        }

        private bool WorkersFilter(object item) => string.IsNullOrEmpty(SearchEntry) || (item as WorkerViewItem).Name.Contains(SearchEntry, StringComparison.OrdinalIgnoreCase);

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string selectedName = ((WorkerViewItem)(sender as CheckBox).DataContext).Name;
            Worker worker = DB.GetWorker(selectedName);
            worker.IsActive = true;
            DB.UpdateWorker(worker);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string selectedName = ((WorkerViewItem)(sender as CheckBox).DataContext).Name;
            Worker worker = DB.GetWorker(selectedName);
            worker.IsActive = false;
            DB.UpdateWorker(worker);
        }

        public static List<WorkerViewItem> GetWorkerItems()
        {
            List<WorkerViewItem> workers = new();
            foreach (Worker worker in DB.GetStaff())
            {
                WorkerViewItem workerItem = new() { Name = worker.Name };
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
                Staff.Add(new WorkerViewItem() { Name = newWorker.Name, Checked = newWorker.IsActive });
                NewWorkerName = string.Empty;
            }
        }

        private static bool IsEmptyName(string name) => string.IsNullOrEmpty(name);

        private static bool IsDublicate(string name) => DB.GetWorker(name) != null;

        private void NewWorkerNameField_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorMessage.Message = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}