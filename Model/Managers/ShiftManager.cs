using Cashbox.Model.Entities;
using Cashbox.Model.Logging;
using Cashbox.Model.Logging.Entities;
using Cashbox.Visu;
using Cashbox.Visu.ViewEntities;
using System;
using System.Collections.Generic;

namespace Cashbox.Model.Managers
{
    public static class ShiftManager
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
            return DB.GetShifts(startPeriod, endPeriod);
        }

        public static List<Shift> GetShifts(string workerName, DateTime startPeriod, DateTime endPeriod)
        {
            int workerId = StaffManager.GetWorker(workerName).Id;
            return DB.GetShifts(workerId, startPeriod, endPeriod);
        }

        public static List<Shift> GetShifts(DateTime date)
        {
            return DB.GetShifts(date);
        }

        public static void AddWorker(this Shift shift, string name)
        {
            Worker worker = DB.GetWorker(name);
            if (!WorkerExists(shift, worker.Id))
                shift.Staff.Add(worker);
        }

        public static void RemoveWorker(this Shift shift, string name)
        {
            Worker worker = DB.GetWorker(name);
            if (WorkerExists(shift, worker.Id))
                shift.Staff.RemoveAt(shift.Staff.FindIndex(w => w.Id == worker.Id));
        }

        public static void UpdateDB(Shift shift)
        {
            shift.User = DB.GetUser(SessionManager.Session.UserId);
            shift.LastModified = DateTime.Now;
            DB.UpdateShift(shift);
            Logger.Log(shift, Logging.MessageType.Update);
        }

        public static void AddToDB(Shift shift)
        {
            shift.User = DB.GetUser(SessionManager.Session.UserId);
            shift.LastModified = DateTime.Now;
            shift.Version++;
            DB.CreateShift(shift);
            Logger.Log(shift, Logging.MessageType.Create);
        }

        public static void RemoveFromDB(DateTime date)
        {
            var removedShift = DB.GetShift(date);
            DB.RemoveShifts(date);
            Logger.Log(removedShift, Logging.MessageType.Delete);
        }

        public static void RemoveFromDB(DateTime date, int version)
        {
            var removedShift = DB.RemoveShift(date, version);
            Logger.Log(removedShift, Logging.MessageType.Delete);
        }

        private static bool WorkerExists(Shift shift, int id)
        {
            return shift?.Staff?.Exists(w => w.Id == id) ?? false;
        }

        public static bool ValidateShift(Shift shift)
        {
            return shift.Staff.Count > 0;
        }

        public static List<WorkerViewItem> GetWorkerViewItems(Shift shift)
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

        public static List<ShiftLogItem> GetExcelShiftCollection(DateTime startPeriod, DateTime endPeriod)
        {
            List<Shift> shifts = DB.GetShifts(startPeriod, endPeriod);
            List<ShiftLogItem> collection = new();
            foreach (Shift item in shifts)
                collection.Add(new ShiftLogItem(item));
            return collection;
        }
    }
}