using Cashbox.Model.Managers;
using Cashbox.Visu.ViewEntities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cashbox.Visu
{
    public partial class StaffView : UserControl, INotifyPropertyChanged
    {
        private string _newWorkerName;
        private string _searchEntry;
        private readonly CollectionView view;
        private ObservableCollection<WorkerViewItem> _staff;

        public ObservableCollection<WorkerViewItem> Staff
        {
            get => _staff ??= new();
            set { _staff = value; OnPropertyChanged(); }
        }

        public string NewWorkerName
        {
            get => _newWorkerName;
            set
            {
                _newWorkerName = value;
                OnPropertyChanged();
                if (value?.Length > 0)
                    ErrorMessage.Message = string.Empty;
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
            UpdateStaff();
            view = (CollectionView)CollectionViewSource.GetDefaultView(Staff);
            view.Filter = WorkersFilter;
        }

        private void UpdateStaff()
        {
            Staff = new(StaffManager.GetAllWorkersViewItems());
        }

        private bool WorkersFilter(object item)
        {
            return string.IsNullOrEmpty(SearchEntry) || (item as WorkerViewItem).Name.Contains(SearchEntry, StringComparison.OrdinalIgnoreCase);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string selectedName = ((WorkerViewItem)(sender as CheckBox).DataContext).Name;
            StaffManager.ActivateWorker(selectedName);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string selectedName = ((WorkerViewItem)(sender as CheckBox).DataContext).Name;
            StaffManager.DeactivateWorker(selectedName);
        }

        private void AddWorker_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StaffManager.AddNewWorker(NewWorkerName);
                UpdateStaff();
                NewWorkerName = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage.Message = ex.Message;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}