using Microsoft.Win32;
using System.Windows;

namespace Cashbox.Services
{
    internal class DefaultDialog : IDialogService
    {
        public string FilePath { get; set; }
        public int SelectedFormat { get; set; }

        private const string filter = "Excel files (*.xlsx)|*.xlsx";
        private const int defaultFilterIndex = 1;        

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = filter,
                FilterIndex = defaultFilterIndex
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                SelectedFormat = openFileDialog.FilterIndex;
                return true;
            }
            return false;
        }

        public bool SaveFileDialog()
        {
            SaveFileDialog openFileDialog = new()
            {
                Filter = filter,
                FilterIndex = defaultFilterIndex
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                SelectedFormat = openFileDialog.FilterIndex;
                return true;
            }
            return false;
        }

        public void ShowMessage(string text)
        {
            MessageBox.Show(text);
        }
    }
}
