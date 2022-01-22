using Cashbox.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model.Repositories
{
    public static class ShiftRepo
    {
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

        public static List<Shift> GetShifts(DateTime begin, DateTime end)
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

        public static List<Shift> GetShifts(int workerId, DateTime begin, DateTime end)
        {
            using ApplicationContext db = new();
            return (from shift in db.Shifts.Include(s => s.Staff).Include(s => s.User).AsEnumerable()
                    where shift.CreatedAt >= begin && shift.CreatedAt <= end && shift.Staff.Exists(w => w.Id == workerId)
                    orderby shift.CreatedAt descending, shift.Version descending
                    group shift by shift.CreatedAt
                          into gr
                    let s = gr.FirstOrDefault()
                    select s).ToList();
        }

        public static List<Shift> GetShifts(DateTime date)
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts.Include(s => s.User).AsEnumerable()
                    where s.CreatedAt == date
                    select s).ToList();
        }

        public static void RemoveShifts(DateTime date)
        {
            using ApplicationContext db = new();
            var shiftsId = (from s in db.Shifts
                            where date.Date == s.CreatedAt
                            select s.Id).ToList();
            foreach (var id in shiftsId)
                db.Shifts.Remove(db.Shifts.Find(id));
            db.SaveChanges();
        }

        public static Shift RemoveShift(DateTime date, int version)
        {
            using ApplicationContext db = new();
            int shiftId = GetShiftId(date, version);
            var entry = db.Shifts.Remove(db.Shifts.Find(shiftId));
            db.SaveChanges();
            return entry.Entity;
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
    }
}
