using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Cashbox.Model.Entities
{
    public class Shift : INotifyPropertyChanged, IEntity
    {
        #region privateFields

        private int _cash;
        private int _terminal;
        private int _expenses;
        private int _startDay;
        private int _endDay;
        private int _handedOver;

        #endregion privateFields

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
        public int Cash
        {
            get => _cash;
            set
            {
                _cash = value;
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Difference));
            }
        }

        /// <summary>
        /// Терминал.
        /// </summary>
        public int Terminal
        {
            get => _terminal;
            set
            {
                _terminal = value;
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Difference));
            }
        }

        /// <summary>
        /// Расходы.
        /// </summary>
        public int Expenses
        {
            get => _expenses;
            set
            {
                _expenses = value;
                OnPropertyChanged(nameof(Difference));
            }
        }

        /// <summary>
        /// Сумма на начало дня.
        /// </summary>
        public int StartDay
        {
            get => _startDay;
            set
            {
                _startDay = value;
                OnPropertyChanged(nameof(Difference));
            }
        }

        /// <summary>
        /// Сумма на конец дня.
        /// </summary>
        public int EndDay
        {
            get => _endDay;
            set
            {
                _endDay = value;
                OnPropertyChanged(nameof(Difference));
            }
        }

        /// <summary>
        /// Сдано денег.
        /// </summary>
        public int HandedOver
        {
            get => _handedOver;
            set
            {
                _handedOver = value;
                OnPropertyChanged(nameof(Difference));
            }
        }

        /// <summary>
        /// Общая выручка.
        /// </summary>
        public int Total => Cash + Terminal;

        /// <summary>
        /// Расхождение.
        /// </summary>
        public int Difference => Cash - Expenses + StartDay - EndDay - HandedOver;

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

        public static Shift Create(User user)
        {
            return new() { User = user, };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}