using Cashbox.Exceptions;
using Cashbox.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Cashbox.Model.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }
        public Permissions Permissions { get; set; }
        public List<Shift> Shifts { get; set; }

        public static User Get(int id)
        {
            return DB.GetUser(id) ?? throw new InvalidNameException("Пользователь не найден");
        }

        public static async Task<List<string>> GetUserNamesAsync()
        {
            var userNames = await DB.GetUserNamesAsync();
            if (userNames.Count == 0)
            {
                List<string> defaultList = new();
                var defaultUsers = XmlService.GetDefaultUsers();
                foreach (User user in defaultUsers)
                {
                    DB.Create(user);
                    defaultList.Add(user.Name);
                }
                return defaultList;
            }
            return userNames;
        }
    }
}