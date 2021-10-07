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
using Cashbox.Model.Managers;
using Cashbox.Exceptions;

namespace Cashbox.Visu
{
    public partial class SalaryLogView : UserControl, INotifyPropertyChanged
    {
        #region privateProperties

        private string _selectedWorkerName;
        private DateTime _start = Formatter.ReturnToFirstDay(DateTime.Today);
        private DateTime _end = Formatter.ReturnToEndOfMonth(DateTime.Today);
        private bool _combinePerMonth;
        private CollectionView salaryLogView;
        private ObservableCollection<SalaryViewItem> _salaryLog;
        private bool _allWorkers;

        #endregion privateProperties

        #region publicProperties

        public ObservableCollection<SalaryViewItem> SalaryLog
        {
            get => _salaryLog ??= new();
            set
            {
                _salaryLog = value;
                salaryLogView = (CollectionView)CollectionViewSource.GetDefaultView(_salaryLog);
                salaryLogView.Filter = WorkerFilter;
                OnPropertyChanged();
            }
        }

        public bool IsWorkersComboBoxEnabled => !AllWorkers;

        public ObservableCollection<string> Staff { get; set; } = new();

        public Permissions Permissions { get; private set; }

        public bool AllWorkers
        {
            get => _allWorkers;
            set { _allWorkers = value; OnPropertyChanged(nameof(IsWorkersComboBoxEnabled)); }
        }

        public DateTime Start
        {
            get => _start;
            set { _start = value > End ? End : value; OnPropertyChanged(); }
        }

        public DateTime End
        {
            get => _end;
            set { _end = value < Start ? Start : value; OnPropertyChanged(); }
        }

        public string SelectedWorkerName
        {
            get => _selectedWorkerName;
            set
            {
                _selectedWorkerName = value;
                ErrorMessage.Message = string.Empty;
                salaryLogView?.Refresh();
            }
        }

        public bool CombinePerMonth
        {
            get => _combinePerMonth;
            set
            {
                _combinePerMonth = value;
                try
                {
                    UpdateSalaryLog();
                }
                catch (InvalidNameException ex)
                {
                    ErrorMessage.Message = ex.Message;
                }
            }
        }

        public MessageProvider ErrorMessage { get; } = new();

        #endregion publicProperties

        public SalaryLogView()
        {
            InitializeComponent();
            DataContext = this;
            Staff = new(StaffManager.GetAllStaff());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void UpdateSalaryLog()
        {
            SalaryLog = AllWorkers
                ? (new(SalaryManager.GetSalaryLog(Start, End, CombinePerMonth)))
                : (new(SalaryManager.GetSalaryLog(SelectedWorkerName, Start, End, CombinePerMonth)));
        }

        private void Button_GetSalaryLog(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateSalaryLog();
            }
            catch (InvalidNameException ex)
            {
                ErrorMessage.Message = ex.Message;
            }
        }

        private bool WorkerFilter(object item)
        {
            return string.IsNullOrEmpty(SelectedWorkerName) || (item as SalaryViewItem).Name == SelectedWorkerName;
        }
    }
}