using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cashbox.Model
{
    public class User
    {
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        public Permissions Permissions { get; set; }
        public List<Worker> Staff { get; set; }
        public List<Shift> Shifts { get; set; }
    }

    public class Worker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public List<Shift> Shifts { get; set; }
        public List<Salary> Salaries { get; set; }
    }

    public class Shift
    {
        public int Id { get; set; }

        /// <summary>
        /// Версия смены.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Дата и время последнего изменения.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Наличные.
        /// </summary>
        public int Cash { get; set; }

        /// <summary>
        /// Терминал.
        /// </summary>
        public int Terminal { get; set; }

        /// <summary>
        /// Расходы.
        /// </summary>
        public int Expenses { get; set; }

        /// <summary>
        /// Сумма на начало дня.
        /// </summary>
        public int StartDay { get; set; }

        /// <summary>
        /// Сумма на конец дня.
        /// </summary>
        public int EndDay { get; set; }

        /// <summary>
        /// Сдано денег.
        /// </summary>
        public int HandedOver { get; set; }

        /// <summary>
        /// Общая выручка.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Расхождение.
        /// </summary>
        public int Difference { get; set; }

        /// <summary>
        /// Комментарий.
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string Comment { get; set; }

        /// <summary>
        /// Сотрудники смены.
        /// </summary>
        public List<Worker> Staff { get; set; } = new();

        public int UserId { get; set; }
        public User User { get; set; }

        public static Shift Create(User user) => new() { /*DateAndTime = DateTime.Now,*/ User = user, };

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
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }

        public int WorkerId { get; set; }
        public Worker Worker { get; set; }
    }
}
