using System;
using System.Collections.Generic;

namespace Cashbox.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Accesses Access { get; set; }

        public enum Accesses { Usual, Administrator }
    }

    public class Shift
    {
        public int Id { get; set; }

        /// <summary>
        /// Дата.
        /// </summary>
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
        public string Comment { get; set; }

        /// <summary>
        /// Сотрудники смены.
        /// </summary>
        public List<User> Users { get; set; }
    }
}
