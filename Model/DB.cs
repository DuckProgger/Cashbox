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
            return db.Staff.Include(w => w.Shifts).Include(w => w.User).ToList();
        }

        public static Worker GetWorker(string name)
        {
            using ApplicationContext db = new();
            return db.Staff.Include(w => w.Shifts).Include(w => w.User).FirstOrDefault(w => w.Name == name);
        }

        public static Worker GetWorker(int id)
        {
            using ApplicationContext db = new();
            return db.Staff.Include(w => w.Shifts).Include(w => w.User).FirstOrDefault(w => w.Id == id);
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
            Shift shift = (from s in db.Shifts.Include(s => s.Staff)
                           where id == s.Id
                           orderby s.Version descending
                           select s).FirstOrDefault();
            return shift;
        }

        public static Shift GetShift(DateTime date)
        {
            using ApplicationContext db = new();
            Shift shift = (from s in db.Shifts.Include(s => s.Staff)
                           where date.Date == s.DateAndTime.Date
                           orderby s.Version descending
                           select s).FirstOrDefault();
            return shift ?? Create(new Shift() { DateAndTime = DateTime.Now, Staff = new() });
        }

        public static Shift GetShift(DateTime date, int version)
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts.Include(s => s.Staff)
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

        public static void CreateShift(Shift shift)
        {
            using ApplicationContext db = new();
            if (!db.Shifts.Contains(shift))
            {
                shift.Version++;
                db.Shifts.Add(shift);
                db.SaveChanges();
            }
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
            //Shift shift = db.Shifts.Find(newShift.Id);
            //Shift shift = GetShift(newShift.Id);

            //shift.Version = newShift.Version;
            //shift.DateAndTime = newShift.DateAndTime;
            //shift.Cash = newShift.Cash;
            //shift.Terminal = newShift.Terminal;
            //shift.Expenses = newShift.Expenses;
            //shift.StartDay = newShift.StartDay;
            //shift.EndDay = newShift.EndDay;
            //shift.HandedOver = newShift.HandedOver;
            //shift.Total = newShift.Total;
            //shift.Difference = newShift.Difference;
            //shift.Comment = newShift.Comment;
            //shift.Staff = new(newShift.Staff);


            //var staff = shift.Staff.Union(newShift.Staff, ).ToList();
            //foreach (var item in staff)
            //    newShift.Staff.Remove(item);

            //foreach (var item in newShift.Staff.ToList())
            //{
            //    if (shift.Staff.Exists(w => w.Id == item.Id))
            //        newShift.Staff.Remove(item);
            //}

            var updatedShift = db.Set<Shift>().Update(newShift);
            //db.Entry(shift).CurrentValues.SetValues(newShift);
            //shift.Staff = new(newShift.Staff);
            db.SaveChanges();
            //return shift;
            return updatedShift.Entity;
        }

        //public static void CreateShift()
        //{
        //    using ApplicationContext db = new();
        //    // Создать смену, если это первая смена за день
        //    if (GetShift(DateTime.Now) == null)
        //    {
        //        db.Shifts.Add(new Shift() { DateAndTime = DateTime.Now, Staff = new() });
        //        db.SaveChanges();
        //    }
        //}

        //public static void SaveNewShift(Shift shift, List<WorkerItem> staff)
        //{
        //    using ApplicationContext db = new();
        //    Shift newShift = new(PrepareShift(shift, staff));
        //    newShift.Version = GetActualShiftVersion() + 1;
        //    newShift.DateAndTime = DateTime.Now;

        //    db.Shifts.Add(newShift);
        //    db.SaveChanges();
        //}

        //public static void EditShift(Shift editedShift, List<WorkerItem> staff)
        //{
        //    using ApplicationContext db = new();
        //    //editedShift = PrepareShift(editedShift, staff);

        //    // Получить из БД нужную смену
        //    int shiftId = GetShiftId(editedShift.DateAndTime.Date, editedShift.Version);
        //    editedShift.Id = shiftId;
        //    Shift shift = db.Shifts.Include(s => s.Staff).Where(s => s.Id == shiftId).FirstOrDefault();

        //    // Присваиваем этой смене все поля изменённой смены
        //    db.Entry(shift).CurrentValues.SetValues(editedShift); // не присваивает свойства ссылочных типов 
        //    shift = PrepareShift(editedShift, staff);
        //    //shift.Staff = editedShift.Staff; // поэтому отдельно присваиваем список работников

        //    db.SaveChanges();
        //}

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

        public static List<WorkerItem> GetWorkerItems(Shift shift)
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

        public static List<object> GetLog(DateTime begin, DateTime end)
        {
            using ApplicationContext db = new();
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

        //public static DB CreateDB()
        //{
        //    return new DB();
        //}

        private static Shift PrepareShift(Shift shift, List<WorkerItem> staff)
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

        private static int GetActualShiftVersion()
        {
            using ApplicationContext db = new();
            return (from s in db.Shifts
                    where s.DateAndTime.Date == DateTime.Now.Date
                    orderby s.Version descending
                    select s.Version).FirstOrDefault();
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


        //public void AddShift(Shift shift)
        //{
        //    db.Shifts.Add(shift);
        //    db.SaveChanges();
        //}

        public static Shift AddWorkerToShift(int shiftId, int workerId)
        {
            using ApplicationContext db = new();
            Shift shift = GetShift(shiftId);
            Worker worker = GetWorker(workerId);
            shift.Staff.Add(worker);
            db.SaveChanges();
            return shift;
        }

        public static Shift RemoveWorkerFromShift(int shiftId, int workerId)
        {
            using ApplicationContext db = new();
            Shift shift = GetShift(shiftId);
            Worker worker = GetWorker(workerId);
            shift.Staff.Remove(worker);
            db.SaveChanges();
            return shift;
        }
    }
}
