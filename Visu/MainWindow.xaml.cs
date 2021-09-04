﻿using Cashbox.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cashbox.Visu
{
    public enum Mode : int { WatchOnly, Edit, New }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User user { get; set; }
        private DB db = new();

        public MainWindow(string userName)
        {
            InitializeComponent();
            user = db.GetUser(userName);
        }

        private void OpenShiftWindow(object sender, RoutedEventArgs e)
        {
            new ShiftWindow(DateTime.Today, Mode.New).Show();
        }

        private void OpenLogWindow(object sender, RoutedEventArgs e)
        {
            new LogWindow(user).Show();
        }
    }
}
