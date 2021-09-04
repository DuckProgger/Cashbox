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

        public virtual Permissions Permissions { get; set; }
        public virtual List<Worker> Staff { get; set; }
    }

    public class Worker
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual User User { get; set; }
        public virtual List<Shift> Shifts { get; set; }
    }

    public class Shift
    {
        public Shift() { }

        public Shift(Shift shift)
        {
            if (shift != null)
            {
                Version = shift.Version;
                DateAndTime = shift.DateAndTime;
                Cash = shift.Cash;
                Terminal = shift.Terminal;
                Expenses = shift.Expenses;
                StartDay = shift.StartDay;
                EndDay = shift.EndDay;
                HandedOver = shift.HandedOver;
                Total = shift.Total;
                Difference = shift.Difference;
                Comment = shift.Comment;
                Staff = shift.Staff != null ? new(shift.Staff) : new();
            }
            else
                DateAndTime = DateTime.Now;

        }

        public int Id { get; set; }

        /// <summary>
        /// Версия смены.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Дата.
        /// </summary>
        //[Column(TypeName = "date")]
        public DateTime DateAndTime { get; set; }

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
        public virtual List<Worker> Staff { get; set; } = new();


        //public Shift DeepCopy()
        //{
        //    Shift shift = (Shift)MemberwiseClone();
        //    shift.Staff = Staff != null ? new(Staff) : new();
        //    return shift;
        //}
    }

    public class Permissions
    {
        public int Id { get; set; }
        public bool IsAdmin { get; set; }

        public virtual User User { get; set; }

        //public Permissions GetAccesses(Accesses access)
        //{
        //    switch (access)
        //    {
        //        case Accesses.Usual:
        //            return new Permissions()
        //            {

        //            };
        //        case Accesses.Administrator:
        //            break;
        //        default:
        //            break;
        //    }
        //    throw new NotImplementedException();
        //}

        //public enum Accesses : int
        //{
        //    Administrator = 0,
        //    Usual = 1,
        //}
    }

    public class WorkerItem
    {
        /// <summary>
        /// Отмечен в смене.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Имя работника.
        /// </summary>
        public string Name { get; set; }
    }
}
