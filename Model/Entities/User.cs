using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashbox.Model.Entities
{
    public class User : IEntity
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        public Permissions Permissions { get; set; }

        //public List<Worker> Staff { get; set; }
        public List<Shift> Shifts { get; set; }
    }
}