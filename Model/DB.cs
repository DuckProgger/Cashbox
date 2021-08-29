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
            return (from user in db.Users
                    select user.Name).ToList();
        }

        public List<User> GetUsers()
        {
            return db.Users.ToList();
        }

        public User GetUser(string userName)
        {
            return (from u in db.Users
                    where u.Name == userName
                    select u).FirstOrDefault();
        }

        public Shift GetShift(DateTime date)
        {
            return (from s in db.Shifts
                    where date.Date == s.Date
                    select s).First();
        }

        public void SaveShift(Shift newShift, List<Worker> workers)
        {
            foreach (var user in GetUsers())
            {
                // Найти работника по имени.
                Worker worker = workers.First(w => w.Name == user.Name);
                // Добавить в смену отмеченного галочкой, если такого ещё нет.
                if (worker.Participated && !newShift.Users.Contains(user))
                    newShift.Users.Add(user);
                // Убрать из смены работника без галочки, если он был.
                else if (!worker.Participated && newShift.Users.Contains(user))
                    newShift.Users.Remove(user);
            }
            Shift curShift = GetShift(newShift.Date);
            curShift = newShift;
            db.SaveChanges();
        }

        public List<Worker> GetWorkers(Shift shift)
        {
            List<Worker> workers = new();
            foreach (User user in GetUsers())
            {
                Worker worker = new()
                {
                    Name = user.Name
                };
                // Поставить галочки работчикам, которые были в смене.
                if (shift.Users.Contains(user))
                    worker.Participated = true;
                workers.Add(worker);
            }
            return workers;
        }

        public void AddShift(Shift shift)
        {
            db.Shifts.Add(shift);
            db.SaveChanges();
        }

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
