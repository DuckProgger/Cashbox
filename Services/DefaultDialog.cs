using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;

namespace Cashbox.Services
{
    internal class DefaultDialog : IDialogService
    {
        public string FilePath { get; set; }
        public int SelectedFormat { get; set; }

        private readonly string filter;
        private const int defaultFilterIndex = 1;

        public DefaultDialog(IFileExtension[] fileServices)
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i < fileServices.Length; i++)
            {
                stringBuilder.Append(GetFilter(fileServices[i]));
                if (i != fileServices.Length - 1)
                    stringBuilder.Append('|');
            }         
            filter = stringBuilder.ToString();
        }

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

        private static string GetFilter(IFileExtension fileExtension)
        {
            //ExtensionAttribute attr = (ExtensionAttribute)Attribute.GetCustomAttribute(typeof(ExcelFileService<ShiftExcelItem>), typeof(ExtensionAttribute));
            return $"{fileExtension.Description} (*.{fileExtension.Extension})|*.{fileExtension.Extension}";
        }
    }
}
