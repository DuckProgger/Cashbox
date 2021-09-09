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
            return (from u in db.Users.Include(u => u.Permissions)/*.Include(u => u.Staff)*/
                    where u.Name == userName
                    select u).FirstOrDefault();
        }

        public static User GetUser(int userId)
        {
            using ApplicationContext db = new();
            return (from u in db.Users.Include(u => u.Permissions)/*.Include(u => u.Staff)*/
                    where u.Id == userId
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
            return (from s in db.Shifts.Include(s => s.Staff).ThenInclude(i => i.User).Include(s => s.User)
                    where date.Date == s.CreatedAt
                    orderby s.Version descending
                    select s).FirstOrDefault();
        }

        public static Shift GetShift(DateTime date, int version)
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts.Include(s => s.Staff).ThenInclude(i => i.User)
                    where date.Date == s.CreatedAt && s.Version == version
                    orderby s.Version descending
                    select s).FirstOrDefault();
        }

        public static int GetShiftId(DateTime date, int version)
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts
                    where date == s.CreatedAt && s.Version == version
                    orderby s.Version descending
                    select s.Id).FirstOrDefault();
        }

        public static Session CreateSession(string userName)
        {
            using ApplicationContext db = new();
            var user = GetUser(userName);
            var session = db.Sessions.Add(new() { UserId = user.Id });
            db.SaveChanges();
            return session.Entity;
        }

        public static void CreateShift(Shift newShift)
        {
            using ApplicationContext db = new();
            Shift shift = newShift.Copy();
            shift.User = db.Users.Find(Global.Session.UserId);
            shift.LastModified = DateTime.Now;
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
            shift.User = db.Users.Find(Global.Session.UserId);
            shift.LastModified = DateTime.Now;
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

        public static void UpdateWorker(Worker newWorker)
        {
            using ApplicationContext db = new();
            var worker = db.Staff.Find(newWorker.Id);
            db.Entry(worker).CurrentValues.SetValues(newWorker);
            db.SaveChanges();
        }

        public static void RemoveShift(DateTime date, int version = 0)
        {
            using ApplicationContext db = new();
            if (version == 0)
            {
                var shiftsId = (from s in db.Shifts
                                where date.Date == s.CreatedAt
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

        public static void RemoveSession(int id)
        {
            using ApplicationContext db = new();
            db.Sessions.Remove(db.Sessions.Find(id));
            db.SaveChanges();
        }

        public static void CaclShift(Shift shift)
        {
            shift.Total = shift.Cash + shift.Terminal;
            shift.Difference = shift.Cash - shift.Expenses + shift.StartDay - shift.EndDay - shift.HandedOver;
        }

        public static List<Shift> GetShiftLog(DateTime begin, DateTime end)
        {
            using ApplicationContext db = new();

            return (from shift in db.Shifts.Include(s => s.Staff).Include(s => s.User).AsEnumerable()
                    where shift.CreatedAt >= begin && shift.CreatedAt <= end
                    orderby shift.CreatedAt descending, shift.Version descending
                    group shift by shift.CreatedAt
                          into gr
                    let s = gr.FirstOrDefault()
                    select s).ToList();
        }

        public static List<Shift> GetShiftVersions(DateTime date)
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts.Include(s => s.User).AsEnumerable()
                    where s.CreatedAt == date
                    select s).ToList();
        }      

        private static bool WorkerExists(Shift shift, int id) => shift.Staff.Exists(w => w.Id == id);

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
