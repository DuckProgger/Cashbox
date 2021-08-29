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
        private readonly ApplicationContext db = new();     

        public async Task<List<string>> GetUserNamesAsync()
        {
            await Task.Run(() => WaitForConnect());
            return GetUserNames();
        }

        public List<string> GetUserNames()
        {
            var userNames = from user in db.Users
                            select user.Name;
            return userNames.ToList();
        }

        public User GetUser(string userName)
        {
            return (from u in db.Users
                    where u.Name == userName
                    select u).FirstOrDefault();
        }

        //public Permissions GetAccesses(string userName)
        //{
        //    return (from user in db.Users
        //            where userName == user.Name
        //            select user.Permissions).First();
        //}

        //public bool CheckPassword(string userName, string enteredPass)
        //{
        //    string rightPass = (from user in db.Users
        //                        where user.Name == userName
        //                        select user.Password).FirstOrDefault();
        //    return rightPass == enteredPass;
        //}

        public void Dispose()
        {
            db.Dispose();
        }

        public static DB CreateDB()
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
