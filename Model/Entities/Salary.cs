using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashbox.Model.Entities
{
    public class Salary : IEntity
    {
        public int Id { get; set; }
        public int Money { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartPeriod { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndPeriod { get; set; }

        public int WorkerId { get; set; }
        public Worker Worker { get; set; }
    }
}