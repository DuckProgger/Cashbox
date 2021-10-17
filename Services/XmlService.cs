using Cashbox.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Cashbox.Services
{
    internal class XmlService
    {
        public static string GetMessageText(int id)
        {
            try
            {
                XDocument xdoc = XDocument.Load("messages.xml");
                var e = xdoc.Element("messages").Elements("message");
                return (from xe in xdoc.Element("messages").Elements("message")
                        where int.Parse(xe.Element("id").Value) == id
                        select xe.Element("text").Value).First();
            }
            catch (FileNotFoundException)
            {
                throw new LogException("Файл не найден");
            }
            catch (Exception)
            {
                throw;
            }
            //XmlSerializer serializer = new(typeof(string));
            //try
            //{
            //    using FileStream fs = new FileStream("people.xml", FileMode.Open);
            //    serializer.Deserialize
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
    }
}