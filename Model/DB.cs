using Cashbox.Model.Entities;
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
        public static T Create<T>(T entity) where T : class
        {
            using ApplicationContext db = new();
            var createdEntity = db.Set<T>().Add(entity);
            db.SaveChanges();
            return createdEntity.Entity;
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
            db.Users.Attach(newShift.User);
            db.Staff.AttachRange(db.Staff.ToList());
            newShift.Id = 0;
            for (int i = 0; i < newShift.Staff.Count; i++)
                newShift.Staff[i] = db.Staff.Find(newShift.Staff[i].Id);
            db.Shifts.Add(newShift);
            db.SaveChanges();
        }

        public static Shift GetPrevShift()
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts
                    orderby s.CreatedAt descending, s.Version descending
                    select s).First();
        }

        public static List<Salary> GetSalaries(int workerId, DateTime start, DateTime end)
        {
            using ApplicationContext db = new();
            return (from s in db.Salaries
                    where s.WorkerId == workerId && s.StartPeriod >= start.Date && s.EndPeriod <= end.Date
                    select s).ToList();
        }

        public static List<Salary> GetSalaries(DateTime start, DateTime end)
        {
            using ApplicationContext db = new();
            return (from s in db.Salaries
                    where s.StartPeriod >= start.Date && s.EndPeriod <= end.Date
                    select s).ToList();
        }

        public static Shift GetShift(int id)
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts.Include(s => s.Staff)
                    where id == s.Id
                    orderby s.Version descending
                    select s).FirstOrDefault();
        }

        public static Shift GetShift(DateTime date)
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts.Include(s => s.Staff).Include(s => s.User)
                    where date.Date == s.CreatedAt
                    orderby s.Version descending
                    select s).FirstOrDefault();
        }

        public static Shift GetShift(DateTime date, int version)
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts.Include(s => s.Staff)
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

        public static List<Worker> GetStaff()
        {
            using ApplicationContext db = new();
            return db.Staff.OrderBy(w => w.Name).ToList();
        }

        public static User GetUser(string userName)
        {
            using ApplicationContext db = new();
            return (from u in db.Users.Include(u => u.Permissions)
                    where u.Name == userName
                    select u).FirstOrDefault();
        }

        public static User GetUser(int userId)
        {
            using ApplicationContext db = new();
            return (from u in db.Users.Include(u => u.Permissions)
                    where u.Id == userId
                    select u).FirstOrDefault();
        }

        public static async Task<List<string>> GetUserNamesAsync()
        {
            using ApplicationContext db = new();
            var userNames = await (from user in db.Users
                                   select user.Name).ToListAsync();
            return userNames;
        }

        public static Worker GetWorker(string name)
        {
            using ApplicationContext db = new();
            return db.Staff.FirstOrDefault(w => w.Name == name);
        }

        public static Worker GetWorker(int id)
        {
            using ApplicationContext db = new();
            return db.Staff.FirstOrDefault(w => w.Id == id);
        }

        public static void RemoveSession(int id)
        {
            using ApplicationContext db = new();
            db.Sessions.Remove(db.Sessions.Find(id));
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

        public static void UpdateShift(Shift newShift)
        {
            using ApplicationContext db = new();
            var dbShift = db.Shifts.Include(s => s.Staff).FirstOrDefault(s => s.Id == newShift.Id);
            db.Entry(dbShift).CurrentValues.SetValues(newShift);
            dbShift.Staff.Clear();
            foreach (var worker in newShift.Staff)
                dbShift.Staff.Add(db.Staff.Find(worker.Id));
            db.SaveChanges();
        }

        public static void UpdateWorker(Worker newWorker)
        {
            using ApplicationContext db = new();
            var worker = db.Staff.Find(newWorker.Id);
            db.Entry(worker).CurrentValues.SetValues(newWorker);
            db.SaveChanges();
        }

        //public static void Update<TEntity>(TEntity entity) where TEntity : class
        //{
        //    using ApplicationContext db = new();

        //    db.Entry(entity).State = EntityState.Modified;
        //    db.SaveChanges();
        //}

        //public static void Insert<TEntity>(TEntity entity) where TEntity : class
        //{
        //    using ApplicationContext db = new();

        //    db.Entry(entity).State = EntityState.Added;
        //    db.SaveChanges();
        //}
    }
}