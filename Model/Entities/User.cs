using Cashbox.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashbox.Model.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }
        public Permissions Permissions { get; set; }
        public List<Shift> Shifts { get; set; }

        public static User Get(int id)
        {
            return DB.GetUser(id) ?? throw new InvalidNameException("Пользователь не найден");
        }
    }
}