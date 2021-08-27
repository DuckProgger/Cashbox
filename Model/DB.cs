using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cashbox.Model
{
    public class DB : IDisposable
    {
        ApplicationContext db = new();

        public DB()
        {
            db.Database.EnsureCreatedAsync();
        }

        public async Task<List<string>> GetUserNamesAsync()
        {
            await Task.Run(() => WaitForConnect());
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

        static public DB CreateDB()
        {
            return new DB();
        }

        private void WaitForConnect()
        {
            while (true)
            {
                Thread.Sleep(500);
                if (db.Database.CanConnect())
                    return;
            }
        }
    }
}
