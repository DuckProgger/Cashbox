using Cashbox.Model.Entities;
using Cashbox.Visu.ViewEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cashbox.Model.Managers
{
    public class ShiftManager
    {
        public static Shift GetShift(DateTime date, int version = 0)
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

        public static List<Shift> GetShifts(DateTime startPeriod, DateTime endPeriod)
        {
            List<Shift> shifts = new();
            foreach (Shift shift in DB.GetShifts(startPeriod, endPeriod))
                shifts.Add(shift);
            return shifts;
        }

        public static List<Shift> GetShifts(string workerName, DateTime startPeriod, DateTime endPeriod)
        {
            List<Shift> shifts = new();
            foreach (Shift shift in DB.GetShifts(startPeriod, endPeriod))
                shifts.Add(shift);
            return shifts;
        }

        public static List<WorkerViewItem> GetWorkerItems(Shift shift)
        {
            List<WorkerViewItem> workers = new();
            foreach (Worker worker in DB.GetStaff())
            {
                if (worker.IsActive)
                {
                    WorkerViewItem workerItem = new() { Name = worker.Name };
                    // Поставить галочки работникам, которые были в смене.
                    if (WorkerExists(shift, worker.Id))
                        workerItem.Checked = true;
                    workers.Add(workerItem);
                }
            }
            return workers;
        }

        public static void AddWorker(Shift shift, string name)
        {
            Worker worker = DB.GetWorker(name);
            if (!WorkerExists(shift, worker.Id))
                shift.Staff.Add(worker);
        }

        public static void RemoveWorker(Shift shift, string name)
        {
            Worker worker = DB.GetWorker(name);
            if (WorkerExists(shift, worker.Id))
                shift.Staff.RemoveAt(shift.Staff.FindIndex(w => w.Id == worker.Id));
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

        public static void Remove(DateTime date)
        {
            DB.RemoveShift(date);
        }

        private static bool WorkerExists(Shift shift, int id)
        {
            return shift?.Staff?.Exists(w => w.Id == id) ?? false;
        }

        public bool IsValidStaffList()
        {
            return Shift.Staff.Count > 0;
        }
    }
}