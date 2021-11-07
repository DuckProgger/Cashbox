using Cashbox.Exceptions;
using Cashbox.Model.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Cashbox.Services
{
    internal class XmlService
    {
        public static string GetMessageText(int id)
        {
            IEnumerable<XElement> items = GetXmlList("messages", "messages", "message");
            return (from xe in items
                    where int.Parse(xe.Element("id").Value) == id
                    select xe.Element("text").Value).First();
        }

        public static IEnumerable<User> GetDefaultUsers()
        {
            IEnumerable<XElement> items = GetXmlList("DefaultUsers", "users", "user");
            return (from xe in items
                    select new User()
                    {
                        Name = xe.Element("name").Value,
                        Permissions = new() { IsAdmin = bool.Parse(xe.Element("isAdmin").Value) }
                    });
        }

        public static IEnumerable<XElement> GetXmlList(string fileName, string listName, string nodeNames)
        {
            try
            {
                XDocument xdoc = XDocument.Load($"{fileName}.xml");
                return xdoc.Element(listName).Elements(nodeNames);
            }
            catch (FileNotFoundException)
            {
                throw new LogException("Файл не найден");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}