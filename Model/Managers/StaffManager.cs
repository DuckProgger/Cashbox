using Cashbox.Exceptions;
using Cashbox.Model.Entities;
using Cashbox.Visu.ViewEntities;
using System.Collections.Generic;

namespace Cashbox.Model.Managers
{
    public class StaffManager
    {
        public static List<string> GetStaffInShifts(List<Shift> shifts)
        {
            List<string> staff = new();
            foreach (Shift shift in shifts)
                foreach (Worker worker in shift.Staff)
                    if (!staff.Contains(worker.Name))
                        staff.Add(worker.Name);
            return staff;
        }

        public static List<string> GetAllStaff()
        {
            List<string> staff = new();
            foreach (Worker worker in DB.GetStaff())
                staff.Add(worker.Name);
            return staff;
        }

        public static Worker GetWorker(string name)
        {
            return DB.GetWorker(name) ?? throw new InvalidNameException("Работник не найден");
        }

        public static void ActivateWorker(string name)
        {
            Worker worker = GetWorker(name);
            worker.IsActive = true;
            DB.UpdateWorker(worker);
        }

        public static void DeactivateWorker(string name)
        {
            Worker worker = GetWorker(name);
            worker.IsActive = false;
            DB.UpdateWorker(worker);
        }

        public static List<WorkerViewItem> GetAllWorkersViewItems()
        {
            List<WorkerViewItem> workers = new();
            foreach (Worker worker in DB.GetStaff())
            {
                WorkerViewItem workerItem = new() { Name = worker.Name };
                // Поставить галочки действующим работникам.
                if (worker.IsActive)
                    workerItem.Checked = true;
                workers.Add(workerItem);
            }
            return workers;
        }

        public static void AddNewWorker(string name)
        {
            if (IsEmptyName(name))
                throw new InvalidNameException("Пустое имя недопустимо");
            else if (IsDublicate(name))
                throw new InvalidNameException("Такой работник уже есть в базе");
            Worker newWorker = new() { Name = name, IsActive = true };
            DB.Create(newWorker);
        }

        private static bool IsEmptyName(string name) => string.IsNullOrEmpty(name);

        private static bool IsDublicate(string name) => DB.GetWorker(name) != null;
    }
}