using Cashbox.Model.Logging.Entities;
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

        //public int UserId { get; set; }
        //public User User { get; set; }
        public List<Shift> Shifts { get; set; }

        public List<Salary> Salaries { get; set; }

        public ILogItem ConvertToLogItem()
        {
            return new WorkerLogItem(this);
        }
    }
}