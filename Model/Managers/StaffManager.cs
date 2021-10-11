using Cashbox.Exceptions;
using Cashbox.Model.Entities;
using Cashbox.Visu.ViewEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model.Managers
{
    public class StaffManager
    {
        //public ObservableCollection<string> Staff { get; set; } = new();

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

        public static Worker GetWorker(string workerName)
        {
            return DB.GetWorker(workerName) ?? throw new InvalidNameException("Работник не найден");
        }
    }
}