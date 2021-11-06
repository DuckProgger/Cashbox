using Cashbox.Model.Entities;
using Cashbox.Model.Logging.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model
{
    public static class Util
    {
        public static string GetDescription<T>(string propertyName)
        {
            AttributeCollection attributes = TypeDescriptor.GetProperties(typeof(T))[propertyName].Attributes;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)attributes[typeof(DescriptionAttribute)];
            return descriptionAttribute.Description;
        }

        public static Dictionary<string, object> GetPropertiesInfo<T>(T obj)
        {
            Dictionary<string, object> dict = new();
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
                dict.Add(GetDescription<T>(prop.Name), prop.GetValue(obj));
            return dict;
        }       
    }
}