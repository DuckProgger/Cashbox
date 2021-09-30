using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cashbox.Model.Entities
{
    public class Worker
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }

        //public int UserId { get; set; }
        //public User User { get; set; }
        public List<Shift> Shifts { get; set; }
        public List<Salary> Salaries { get; set; }
    }
}
