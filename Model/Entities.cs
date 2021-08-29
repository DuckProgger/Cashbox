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
        //public virtual Permissions.Accesses Access { get; set; }

        public virtual Permissions Permissions { get; set; }
        public virtual List<Shift> Shifts { get; set; }


        //public string Password { get; set; }
        //public Accesses Access { get; set; }

        //public enum Accesses { Usual, Administrator }
    }    

    public class Shift
    {
        public int Id { get; set; }

        /// <summary>
        /// Дата.
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Наличные.
        /// </summary>
        public decimal Cash { get; set; }

        /// <summary>
        /// Терминал.
        /// </summary>
        public decimal Terminal { get; set; }

        /// <summary>
        /// Расходы.
        /// </summary>
        public decimal Expenses { get; set; }

        /// <summary>
        /// Сумма на начало дня.
        /// </summary>
        public decimal StartDay { get; set; }

        /// <summary>
        /// Сумма на конец дня.
        /// </summary>
        public decimal EndDay { get; set; }

        /// <summary>
        /// Сдано денег.
        /// </summary>
        public decimal HandedOver { get; set; }

        /// <summary>
        /// Общая выручка.
        /// </summary>
        //public decimal Total { get; set; }

        /// <summary>
        /// Расхождение.
        /// </summary>
        //public decimal Difference { get; set; }

        /// <summary>
        /// Комментарий.
        /// </summary>
        [Column(TypeName = "nvarchar(200)")]
        public string Comment { get; set; }

        /// <summary>
        /// Сотрудники смены.
        /// </summary>
        public virtual List<User> Users { get; set; }
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
}
