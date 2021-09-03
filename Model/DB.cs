﻿using System;
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

        public List<Worker> GetStaff()
        {
            return db.Staff.ToList();
        }

        public Worker GetWorker(string name)
        {
            return db.Staff.FirstOrDefault(w => w.Name == name);
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

        public void SaveNewShift(Shift shift, List<WorkerItem> staff)
        {
            Shift newShift = new(shift);
            foreach (var wi in staff)
            {
                // Найти работника по имени. 
                Worker worker = GetWorker(wi.Name);

                // Добавить в смену отмеченного галочкой, если такого ещё нет.
                if (worker != null)
                {
                    if (wi.Checked && !newShift.Staff.Contains(worker))
                        newShift.Staff.Add(worker);
                    // Убрать из смены работника без галочки, если он был.
                    else if (!wi.Checked && newShift.Staff.Contains(worker))
                        newShift.Staff.Remove(worker);
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

        public List<WorkerItem> GetWorkerItems(Shift shift)
        {
            List<WorkerItem> workers = new();
            foreach (Worker user in GetStaff())
            {
                WorkerItem worker = new()
                {
                    Name = user.Name
                };
                // Поставить галочки работчикам, которые были в смене.
                if (shift.Staff != null && shift.Staff.Contains(user))
                {
                    worker.Checked = true;
                }
                workers.Add(worker);
            }
            return workers;
        }   

        public List<object> GetLog(DateTime begin, DateTime end)
        {
            List<object> logItems = new();

            var items = from shift in db.Shifts.AsEnumerable()
                        where shift.Date >= begin && shift.Date <= end
                        orderby shift.Date descending, shift.Version descending
                        group shift by shift.Date
                         into gr
                        let s = gr.FirstOrDefault()
                        select new
                        {
                            s.Date,
                            Staff = TransformWorkersToString(s.Staff),
                            s.Total,
                            s.Difference
                        };

            foreach (var item in items)
                logItems.Add(item);
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




        //public void AddShift(Shift shift)
        //{
        //    db.Shifts.Add(shift);
        //    db.SaveChanges();
        //}

        //public void AddWorkerToShift(DateTime shiftDate, WorkerItem workerItem)
        //{
        //    Shift shift = GetShift(shiftDate);
        //    Worker worker = new() { Name = workerItem.Name };
        //    shift.Staff.Add(worker);
        //    //db.SaveChanges();
        //}

        //public void RemoveWorkerFromShift(DateTime shiftDate, WorkerItem workerItem)
        //{
        //    Shift shift = GetShift(shiftDate);
        //    Worker worker = shift.Staff.First(w => w.Name == workerItem.Name);
        //    shift.Staff.Remove(worker);
        //    //db.SaveChanges();
        //}
    }
}
