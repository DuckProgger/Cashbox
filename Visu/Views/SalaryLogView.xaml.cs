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
        private CollectionView salariesView;
        private ObservableCollection<SalaryViewItem> _salaries;
        private bool _allWorkers = true;

        #endregion privateProperties

        #region publicProperties

        public int TotalSalary
        {
            get
            {
                try
                {
                    if (Salaries?.Count > 0)
                        return string.IsNullOrEmpty(SelectedWorkerName)
                         ? SalaryManager.GetTotalSalary(Start, End)
                         : SalaryManager.GetTotalSalary(SelectedWorkerName, Start, End);
                    else
                        return 0;
                }
                catch (InvalidNameException ex)
                {
                    ErrorMessage.Message = ex.Message;
                    return 0;
                }
            }
        }

        public ObservableCollection<SalaryViewItem> Salaries
        {
            get => _salaries ??= new();
            set
            {
                _salaries = value;
                salariesView = (CollectionView)CollectionViewSource.GetDefaultView(_salaries);
                salariesView.Filter = WorkerFilter;
                salariesView?.Refresh();
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalSalary));
            }
        }

        public bool IsWorkersComboBoxEnabled => !AllWorkers;

        public ObservableCollection<string> Staff { get; set; } = new();

        public Permissions Permissions { get; private set; }

        public bool AllWorkers
        {
            get => _allWorkers;
            set
            {
                _allWorkers = value;
                SelectedWorkerName = string.Empty;
                OnPropertyChanged(nameof(IsWorkersComboBoxEnabled));
                OnPropertyChanged(nameof(TotalSalary));
            }
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
                salariesView?.Refresh();
                OnPropertyChanged(nameof(TotalSalary));
                OnPropertyChanged();
            }
        }

        public bool CombinePerMonth
        {
            get => _combinePerMonth;
            set
            {
                _combinePerMonth = value;
                GetSalaryLog();
            }
        }

        public MessageProvider ErrorMessage { get; } = new(true);

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

        private void GetSalaryLog()
        {
            try
            {
                Salaries = new(SalaryManager.GetSalaryLog(Start, End, CombinePerMonth));
            }
            catch (InvalidNameException ex)
            {
                ErrorMessage.Message = ex.Message;
            }
        }

        private void Button_GetSalaryLog(object sender, RoutedEventArgs e)
        {
            GetSalaryLog();
        }

        private bool WorkerFilter(object item)
        {
            return string.IsNullOrEmpty(SelectedWorkerName) || (item as SalaryViewItem).Name == SelectedWorkerName;
        }
    }
}