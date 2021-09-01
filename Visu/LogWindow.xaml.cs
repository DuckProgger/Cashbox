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
    /// Логика взаимодействия для LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        private DB db = new();

        public ObservableCollection<LogItem> Log { get; set; } = new();
        public DateTime Begin { get; set; } = DateTime.Today;
        public DateTime End { get; set; } = DateTime.Today;

        public LogWindow()
        {
            InitializeComponent();
            DataContext = this;
        }


        private void Button_GetLog(object sender, RoutedEventArgs e)
        {
            Log.Clear();
            foreach (var item in db.GetLog(Begin, End))
                Log.Add(item);
        }
    }
}
