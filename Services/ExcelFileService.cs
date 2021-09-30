using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Cashbox.Services
{
    public class ExcelFileService<T> : IFileService<T>
    {
        public string Description => "Excel files";
        public string Extension => "xlsx";

        public void SaveFile(string filename, List<T> data)
        {
            const string workSheetName = "Лист";
            using ExcelPackage package = new(new FileInfo(filename));
            ExcelWorksheet excelWorksheet = package.Workbook.Worksheets[workSheetName];
            if (excelWorksheet == null)
            {
                List<string> descriptions = GetDescriptions();
                excelWorksheet = package.Workbook.Worksheets.Add(workSheetName);
                for (int i = 0; i < descriptions.Count; i++)
                    excelWorksheet.Cells[1, i + 1].Value = descriptions[i];
                excelWorksheet.Cells["A2"].LoadFromCollection(data);
            }
            package.Save();
        }

        private static string GetDescription(string propertyName)
        {
            AttributeCollection attributes = TypeDescriptor.GetProperties(typeof(T))[propertyName].Attributes;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)attributes[typeof(DescriptionAttribute)];
            return descriptionAttribute.Description;
        }

        private static List<string> GetDescriptions()
        {
            List<string> list = new();
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                string description = GetDescription(prop.Name);
                if (!string.IsNullOrEmpty(description))
                    list.Add(GetDescription(prop.Name));
            }
            return list;
        }
    }
}
