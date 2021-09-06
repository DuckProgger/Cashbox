using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cashbox.Model
{
    public static class DB
    {
        public static async Task<List<string>> GetUserNamesAsync()
        {
            using ApplicationContext db = new();
            await Task.Run(() => WaitForConnect(db));
            return GetUserNames();
        }

        public static List<string> GetUserNames()
        {
            using ApplicationContext db = new();
            return (from user in db.Users
                    select user.Name).ToList();
        }

        public static List<Worker> GetStaff()
        {
            using ApplicationContext db = new();
            return db.Staff/*.Include(w => w.Shifts).Include(w => w.User)*/.ToList();
        }

        public static Worker GetWorker(string name)
        {
            using ApplicationContext db = new();
            return db.Staff/*.Include(w => w.Shifts)*//*.Include(w => w.User)*/.FirstOrDefault(w => w.Name == name);
        }

        public static Worker GetWorker(int id)
        {
            using ApplicationContext db = new();
            return db.Staff/*.Include(w => w.Shifts)*//*.Include(w => w.User)*/.FirstOrDefault(w => w.Id == id);
        }

        public static User GetUser(string userName)
        {
            using ApplicationContext db = new();
            return (from u in db.Users.Include(u => u.Permissions).Include(u => u.Staff)
                    where u.Name == userName
                    select u).FirstOrDefault();
        }

        public static Shift GetShift(int id)
        {
            using ApplicationContext db = new();
            Shift shift = (from s in db.Shifts.Include(s => s.Staff).ThenInclude(i => i.User)
                           where id == s.Id
                           orderby s.Version descending
                           select s).FirstOrDefault();
            return shift;
        }

        public static Shift GetShift(DateTime date)
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts.Include(s => s.Staff)/*.ThenInclude(i => i.User)*/.Include(s => s.User)
                    where date.Date == s.DateAndTime.Date
                    orderby s.Version descending
                    select s).FirstOrDefault();
        }

        public static Shift GetShift(DateTime date, int version)
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts.Include(s => s.Staff).ThenInclude(i => i.User)
                    where date.Date == s.DateAndTime.Date && s.Version == version
                    orderby s.Version descending
                    select s).FirstOrDefault();
        }

        public static int GetShiftId(DateTime date, int version)
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts
                    where date == s.DateAndTime.Date && s.Version == version
                    orderby s.Version descending
                    select s.Id).FirstOrDefault();
        }

        public static void CreateShift(Shift newShift)
        {
            using ApplicationContext db = new();
            //Shift shift = new(newShift);
            Shift shift = newShift.DeepCopy();

            shift.Version++;
            foreach (var worker in db.Staff.ToList())
                // Добавить в смену работника, если он есть в новой смене.
                if (newShift.Staff.Exists(w => w.Id == worker.Id))
                    shift.Staff.Add(worker);
            db.Shifts.Add(shift);

            db.SaveChanges();
        }

        public static T Create<T>(T entity) where T : class
        {
            using ApplicationContext db = new();
            var createdEntity = db.Set<T>().Add(entity);
            db.SaveChanges();
            return createdEntity.Entity;
        }

        public static Shift UpdateShift(Shift newShift)
        {
            using ApplicationContext db = new();
            var shift = db.Shifts.Include(s => s.Staff).FirstOrDefault(s => s.Id == newShift.Id);
            db.Entry(shift).CurrentValues.SetValues(newShift);
            foreach (var worker in db.Staff.ToList())
            {
                // Добавить в смену работника, если он есть в новой смене и его нет в старой версии смены.
                if (WorkerExists(newShift, worker.Id) && !WorkerExists(shift, worker.Id))
                    shift.Staff.Add(worker);
                // Убрать из смены работника, если его нет в новой смене и он есть в старой версии смены.
                else if (!WorkerExists(newShift, worker.Id) && WorkerExists(shift, worker.Id))
                    shift.Staff.Remove(worker);
            }
            db.SaveChanges();
            return shift;
        }

        public static void RemoveShift(DateTime date, int version = 0)
        {
            using ApplicationContext db = new();
            if (version == 0)
            {
                var shiftsId = (from s in db.Shifts
                                where date.Date == s.DateAndTime.Date
                                select s.Id).ToList();
                foreach (var id in shiftsId)
                    db.Shifts.Remove(db.Shifts.Find(id));
            }
            else
            {
                int shiftId = GetShiftId(date, version);
                db.Shifts.Remove(db.Shifts.Find(shiftId));
            }
            db.SaveChanges();
        }

        public static void CaclShift(Shift shift)
        {
            shift.Total = shift.Cash + shift.Terminal;
            shift.Difference = shift.Cash - shift.Expenses + shift.StartDay - shift.EndDay - shift.HandedOver;
        }

        public static List<object> GetLog(DateTime begin, DateTime end)
        {
            using ApplicationContext db = new();
            List<object> logItems = new();

            var items = from shift in db.Shifts.Include(s => s.Staff).AsEnumerable()
                        where shift.DateAndTime.Date >= begin && shift.DateAndTime.Date <= end
                        orderby shift.DateAndTime descending, shift.Version descending
                        group shift by shift.DateAndTime.Date
                         into gr
                        let s = gr.FirstOrDefault()
                        select new
                        {
                            Date = s.DateAndTime,
                            Staff = TransformWorkersToString(s.Staff),
                            s.Total,
                            s.Difference
                        };

            foreach (var item in items)
                logItems.Add(item);
            return logItems;
        }

        public static List<object> GetShiftVersionHistory(DateTime date)
        {
            using ApplicationContext db = new();
            List<object> logItems = new();
            var items = from s in db.Shifts.AsEnumerable()
                        where s.DateAndTime.Date == date
                        select new { Time = s.DateAndTime, s.Version };

            foreach (var item in items)
                logItems.Add(item);
            return logItems;
        }

        private static bool WorkerExists(Shift shift, int id) => shift.Staff.Exists(w => w.Id == id);

        private static string TransformWorkersToString(List<Worker> workers)
        {
            StringBuilder builder = new();
            for (int i = 0; i < workers.Count; i++)
            {
                builder.Append(workers[i].Name);
                if (i < workers.Count - 1)
                    builder.Append(',');
            }
            return builder.ToString();
        }

        private static void WaitForConnect(ApplicationContext db)
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
