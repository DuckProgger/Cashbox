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
            return new Shift((from s in db.Shifts
                              where date.Date == s.Date
                              orderby s.Version descending
                              select s).FirstOrDefault());
        }

        public void CreateShift()
        {
            if (GetShift(DateTime.Now) == null)
            {
                db.Shifts.Add(new Shift() { Date = DateTime.Now });
                db.SaveChanges();
            }
        }

        public void SaveShift(Shift shift, List<Worker> workers)
        {
            Shift newShift = new(shift);
            foreach (var user in GetUsers())
            {
                // Найти работника по имени.
                Worker worker = workers.FirstOrDefault(w => w.Name == user.Name);
                // Добавить в смену отмеченного галочкой, если такого ещё нет.
                if (worker != null)
                {
                    if (worker.Participated && !newShift.Users.Contains(user))
                        newShift.Users.Add(user);
                    // Убрать из смены работника без галочки, если он был.
                    else if (!worker.Participated && newShift.Users.Contains(user))
                        newShift.Users.Remove(user);
                }                
            }

            newShift.Version = GetActualShiftVersion() + 1;
            db.Shifts.Add(newShift);
            db.SaveChanges();
        }

        public static void CaclShift(Shift shift)
        {
            shift.Total = shift.Cash + shift.Terminal;
            shift.Difference = shift.Cash - shift.Expenses + shift.StartDay - shift.EndDay - shift.HandedOver;
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
                if (shift.Users != null && shift.Users.Contains(user))
                {
                    worker.Participated = true;
                    workers.Add(worker);
                }
            }
            return workers;
        }

        public void AddShift(Shift shift)
        {
            db.Shifts.Add(shift);
            db.SaveChanges();
        }

        public List<LogItem> GetLog(DateTime begin, DateTime end)
        {
            List<LogItem> logItems = new();

            var shifts = from shift in db.Shifts.AsEnumerable()
                         where shift.Date >= begin && shift.Date <= end
                         orderby shift.Date descending, shift.Version descending
                         group shift by shift.Date
                         into gr
                         let s = gr.FirstOrDefault()
                         select new
                         {
                             s.Date,
                             s.Users,
                             s.Total,
                             s.Difference
                         };

            foreach (var shift in shifts)
            {
                LogItem logItem = new()
                {
                    Date = shift.Date.Date,
                    TotalCash = shift.Total,
                    Difference = shift.Difference,
                    Workers = TransformUsersToString(shift.Users)
                };
                logItems.Add(logItem);
            }

            return logItems;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public static DB CreateDB()
        {
            return new DB();
        }

        private static string TransformUsersToString(List<User> users)
        {
            StringBuilder builder = new();
            for (int i = 0; i < users.Count; i++)
            {
                builder.Append(users[i].Name);
                if (i < users.Count - 1)
                    builder.Append(',');
            }
            return builder.ToString();
        }

        private int GetActualShiftVersion()
        {
            return (from s in db.Shifts
                    where s.Date == DateTime.Now.Date
                    orderby s.Version descending
                    select s.Version).FirstOrDefault();
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
