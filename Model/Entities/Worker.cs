﻿using Cashbox.Exceptions;
using Cashbox.Model.Logging;
using Cashbox.Model.Logging.Entities;
using Cashbox.Model.Repositories;
using Cashbox.Visu.ViewEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cashbox.Model.Entities
{
    public class Worker : ILogged
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<Shift> Shifts { get; set; }
        public List<Salary> Salaries { get; set; }

        public ILogItem ConvertToLogItem() => (WorkerLogItem)this;

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
            foreach (Worker worker in WorkerRepo.GetStaff())
                staff.Add(worker.Name);
            return staff;
        }

        public static Worker Get(string name)
        {
            return WorkerRepo.GetWorker(name) ?? throw new InvalidNameException("Работник не найден");
        }

        public static Worker GetWorker(int id)
        {
            return WorkerRepo.GetWorker(id) ?? throw new InvalidNameException("Работник не найден");
        }

        public static void Activate(string name)
        {
            Worker worker = Get(name);
            worker.IsActive = true;
            WorkerRepo.UpdateWorker(worker);
        }

        public static void Deactivate(string name)
        {
            Worker worker = Get(name);
            worker.IsActive = false;
            WorkerRepo.UpdateWorker(worker);
        }

        public static List<WorkerViewItem> GetAllWorkersViewItems()
        {
            List<WorkerViewItem> workers = new();
            foreach (Worker worker in WorkerRepo.GetStaff())
            {
                WorkerViewItem workerItem = new() { Name = worker.Name };
                // Поставить галочки действующим работникам.
                if (worker.IsActive)
                    workerItem.Checked = true;
                workers.Add(workerItem);
            }
            return workers;
        }

        public static void AddNew(string name)
        {
            if (IsEmptyName(name))
                throw new InvalidNameException("Пустое имя недопустимо");
            else if (IsDublicate(name))
                throw new InvalidNameException("Такой работник уже есть в базе");
            Worker newWorker = new() { Name = name, IsActive = true };
            CommonRepo.Create(newWorker);
            Logger.Log(newWorker, MessageType.Create);
        }

        private static bool IsEmptyName(string name) => string.IsNullOrEmpty(name);

        private static bool IsDublicate(string name) => WorkerRepo.GetWorker(name) != null;
    }
}