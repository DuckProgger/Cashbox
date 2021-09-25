using OfficeOpenXml.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashbox.Model
{
    public class User
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        public Permissions Permissions { get; set; }
        //public List<Worker> Staff { get; set; }
        public List<Shift> Shifts { get; set; }
    }

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

    public class Shift
    {
        [EpplusIgnore]
        public int Id { get; set; }

        [Description("Версия смены")]
        public int Version { get; set; }
        
        [Description("Дата создания")]
        [Column(TypeName = "date")]
        public DateTime CreatedAt { get; set; }

        [Description("Дата и время последнего изменения")]
        public DateTime LastModified { get; set; }

        [Description("Наличные")]
        public int Cash { get; set; }
      
        [Description("Терминал")]
        public int Terminal { get; set; }

        [Description("Расходы")]
        public int Expenses { get; set; }

        [Description("Сумма на начало дня")]
        public int StartDay { get; set; }

        [Description("Сумма на конец дня")]
        public int EndDay { get; set; }

        [Description("Сдано денег")]
        public int HandedOver { get; set; }

        [Description("Общая выручка")]
        public int Total { get; set; }

        [Description("Расхождение")]
        public int Difference { get; set; }

        [Description("Комментарий")]
        [Column(TypeName = "nvarchar(200)")]
        public string Comment { get; set; }

        /// <summary>
        /// Сотрудники смены.
        /// </summary>
        [EpplusIgnore]
        public List<Worker> Staff { get; set; } = new();
        [EpplusIgnore]
        public int UserId { get; set; }
        [EpplusIgnore]
        public User User { get; set; }

        public static Shift Create(User user) => new() { User = user, };

        //public Shift Copy()
        //{
        //    Shift shift = (Shift)MemberwiseClone();
        //    shift.Id = default;
        //    //shift.User = new User() { Id = User.Id, Name = User.Name };
        //    //shift.Staff = Staff != null ? new(Staff) : new();
        //    return shift;
        //}

        public Shift Copy()
        {
            return new Shift
            {
                Version = Version,
                CreatedAt = CreatedAt,
                Cash = Cash,
                Terminal = Terminal,
                Expenses = Expenses,
                StartDay = StartDay,
                EndDay = EndDay,
                HandedOver = HandedOver,
                Total = Total,
                Difference = Difference,
                Comment = Comment
            };
        }

    }

    public class Permissions
    {
        public int Id { get; set; }
        public bool IsAdmin { get; set; }

        public User User { get; set; }

        public static Permissions GetAccesses(int userId)
        {
            var user = DB.GetUser(userId);
            return user.Permissions;
        }
    }

    public class Session
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }

    public class Salary
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
