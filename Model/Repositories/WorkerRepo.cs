using Cashbox.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model.Repositories
{
    public static class WorkerRepo
    {
        public static List<Worker> GetStaff()
        {
            using ApplicationContext db = new();
            return db.Staff.OrderBy(w => w.Name).ToList();
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

        public static void UpdateWorker(Worker newWorker)
        {
            using ApplicationContext db = new();
            var worker = db.Staff.Find(newWorker.Id);
            db.Entry(worker).CurrentValues.SetValues(newWorker);
            db.SaveChanges();
        }

    }
}
