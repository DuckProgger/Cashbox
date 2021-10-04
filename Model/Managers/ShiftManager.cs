using Cashbox.Model.Entities;
using Cashbox.Visu.ViewEntities;
using System;
using System.Collections.ObjectModel;

namespace Cashbox.Model.Managers
{
    public class ShiftManager
    {
        public ShiftManager(DateTime date, int version = 0)
        {
            Shift = GetShift(date, version);
            Staff = GetWorkerItems();
        }

        public Shift Shift { get; set; }
        public ObservableCollection<WorkerViewItem> Staff { get; private set; }

        private static Shift GetShift(DateTime date, int version = 0)
        {
            Shift shift;
            if (version == 0)
            {
                shift = DB.GetShift(date);
                if (shift == null)
                {
                    shift = Shift.Create(DB.GetUser(SessionManager.Session.UserId));
                    try
                    {
                        shift.StartDay = DB.GetPrevShift().EndDay;
                    }
                    catch (InvalidOperationException)
                    {
                        shift.StartDay = 0;
                    }
                }
            }
            else
                shift = DB.GetShift(date, version);
            return shift;
        }

        private ObservableCollection<WorkerViewItem> GetWorkerItems()
        {
            ObservableCollection<WorkerViewItem> workers = new();
            foreach (Worker worker in DB.GetStaff())
            {
                if (worker.IsActive)
                {
                    WorkerViewItem workerItem = new() { Name = worker.Name };
                    // Поставить галочки работникам, которые были в смене.
                    if (WorkerExists(worker.Id))
                        workerItem.Checked = true;
                    workers.Add(workerItem);
                }
            }
            return workers;
        }

        public void AddWorker(string name)
        {
            Worker worker = DB.GetWorker(name);
            if (!WorkerExists(worker.Id))
                Shift.Staff.Add(worker);
        }

        public void RemoveWorker(string name)
        {
            Worker worker = DB.GetWorker(name);
            if (WorkerExists(worker.Id))
                Shift.Staff.RemoveAt(Shift.Staff.FindIndex(w => w.Id == worker.Id));
        }

        public void UpdateDB()
        {
            Shift.User = DB.GetUser(SessionManager.Session.UserId);
            Shift.LastModified = DateTime.Now;
            DB.UpdateShift(Shift);
        }

        public void AddToDB()
        {
            Shift.User = DB.GetUser(SessionManager.Session.UserId);
            Shift.LastModified = DateTime.Now;
            Shift.Version++;
            DB.CreateShift(Shift);
        }

        public static void RemoveFromDB(DateTime date)
        {
            DB.RemoveShift(date);
        }

        private bool WorkerExists(int id)
        {
            return Shift?.Staff?.Exists(w => w.Id == id) ?? false;
        }

        public bool IsValidStaffList()
        {
            return Shift.Staff.Count > 0;
        }
    }
}