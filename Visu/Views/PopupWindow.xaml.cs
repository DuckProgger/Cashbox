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
    /// <summary>
    /// Логика взаимодействия для PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        public object CurrentView { get; }

        public PopupWindow(object content)
        {
            InitializeComponent();
            DataContext = this;
            CurrentView = content;
        }
    }
}
