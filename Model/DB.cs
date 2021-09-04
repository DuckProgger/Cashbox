using Microsoft.EntityFrameworkCore;
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
            return (from s in db.Shifts
                    where date.Date == s.DateAndTime.Date
                    orderby s.Version descending
                    select s).FirstOrDefault();
        }

        public Shift GetShiftByVersion(DateTime date, int version)
        {
            return (from s in db.Shifts
                    where date.Date == s.DateAndTime.Date && s.Version == version
                    orderby s.Version descending
                    select s).FirstOrDefault();
        }

        public void CreateShift()
        {
            // Создать смену, если это первая смена за день
            if (GetShift(DateTime.Now) == null)
            {
                db.Shifts.Add(new Shift() { DateAndTime = DateTime.Now });
                db.SaveChanges();
            }
        }

        public void SaveNewShift(Shift shift, List<WorkerItem> staff)
        {
            Shift newShift = new(PrepareShift(shift, staff));
            newShift.Version = GetActualShiftVersion() + 1;
            newShift.DateAndTime = DateTime.Now;

            db.Shifts.Add(newShift);
            db.SaveChanges();
        }

        public void EditShift(Shift editedShift, List<WorkerItem> staff)
        {
            editedShift = PrepareShift(editedShift, staff);
            // Получить из БД нужную смену
            Shift shift = GetShiftByVersion(editedShift.DateAndTime.Date, editedShift.Version);
            // Присваиваем этой смене все поля изменённой смены
            shift.Cash = editedShift.Cash;
            shift.Comment = editedShift.Comment;
            shift.DateAndTime = editedShift.DateAndTime;
            shift.Difference = editedShift.Difference;
            shift.EndDay = editedShift.EndDay;
            shift.Expenses = editedShift.Expenses;
            shift.HandedOver = editedShift.HandedOver;
            shift.Staff = editedShift.Staff;
            shift.StartDay = editedShift.StartDay;
            shift.Terminal = editedShift.Terminal;
            shift.Total = editedShift.Total;
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
            foreach (Worker worker in GetStaff())
            {
                WorkerItem workerItem = new()
                {
                    Name = worker.Name
                };
                // Поставить галочки работчикам, которые были в смене.
                if (shift.Staff != null && IsWorkerExists(shift, worker.Id))
                {
                    workerItem.Checked = true;
                }
                workers.Add(workerItem);
            }
            return workers;
        }

        public List<object> GetLog(DateTime begin, DateTime end)
        {
            List<object> logItems = new();

            var items = from shift in db.Shifts.AsEnumerable()
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

        public List<object> GetShiftVersionHistory(DateTime date)
        {
            List<object> logItems = new();
            var items = from s in db.Shifts.AsEnumerable()
                        where s.DateAndTime.Date == date
                        select new { Time = s.DateAndTime, s.Version };

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

        private Shift PrepareShift(Shift shift, List<WorkerItem> staff)
        {
            foreach (var wi in staff)
            {
                // Найти работника по имени. 
                Worker worker = GetWorker(wi.Name);
                if (worker != null)
                {
                    // Добавить в смену отмеченного галочкой, если такого ещё нет.
                    if (wi.Checked && !IsWorkerExists(shift, worker.Id))
                        shift.Staff.Add(worker);
                    // Убрать из смены работника без галочки, если он был.
                    else if (!wi.Checked && IsWorkerExists(shift, worker.Id))
                        shift.Staff.Remove(worker);
                }
            }
            return shift;
        }

        private static bool IsWorkerExists(Shift shift, int id) => shift.Staff.Exists(w => w.Id == id);

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
                    where s.DateAndTime.Date == DateTime.Now.Date
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
