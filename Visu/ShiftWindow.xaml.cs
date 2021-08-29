using System;
using System.Collections.Generic;
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
        public List<string> Workers { get; private set; }

        public ShiftWindow()
        {
            InitializeComponent();
            //WorkersTable.ItemsSource
        }
    }

    public class Worker
    {
        public string Name { get; set; }
        public bool Participated { get; set; }
    }
}
