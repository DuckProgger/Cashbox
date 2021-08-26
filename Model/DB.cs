using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model
{
    public class DB : IDisposable
    {
        ApplicationContext db = new();

        public DB()
        {
            db.Database.EnsureCreated();
        }

        public List<string> GetUserNames()
        {
            // Получение всех пользователей из БД
            var userNames = from user in db.Users
                            select user.Name;
            return userNames.ToList();
        }

        public bool CheckPassword(string userName, string enteredPass)
        {     
            string rightPass = (from user in db.Users
                                where user.Name == userName
                                select user.Password).FirstOrDefault();
            return rightPass == enteredPass;
        }

        public void Dispose()
        {
            db.Dispose(); 
        }
    }
}
