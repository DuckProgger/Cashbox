using Cashbox.Model.Logging;
using Cashbox.Model.Logging.Entities;
using Cashbox.Visu.ViewEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Cashbox.Model.Entities
{
    public class Shift : INotifyPropertyChanged, ILogged
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public ILogItem ConvertToLogItem()
        {
            return new ShiftLogItem(this);
        }

        private static Shift Create(User user)
        {
            return new() { User = user, };
        }

        public static Shift GetShift(DateTime date, int version = 0)
        {
            Shift shift;
            if (version == 0)
            {
                shift = DB.GetShift(date);
                if (shift == null)
                {
                    shift = Create(DB.GetUser(Session.Current.UserId));
                    try
                    {
                        shift.StartDay = DB.GetPrevShift().EndDay;
                    }
                    catch (InvalidOperationException)
                    {
                        shift.StartDay = 0;
                    }
                }
            }
            else
            {
                shift = DB.GetShift(date, version);
            }

            return shift;
        }

        public static List<Shift> GetShifts(DateTime startPeriod, DateTime endPeriod)
        {
            return DB.GetShifts(startPeriod, endPeriod);
        }

        public static List<Shift> GetShifts(string workerName, DateTime startPeriod, DateTime endPeriod)
        {
            int workerId = Worker.Get(workerName).Id;
            return DB.GetShifts(workerId, startPeriod, endPeriod);
        }

        public static List<Shift> GetShifts(DateTime date)
        {
            return DB.GetShifts(date);
        }

        public void AddWorker(string name)
        {
            Worker worker = DB.GetWorker(name);
            if (!Exists(this, worker.Id))
                Staff.Add(worker);
        }

        public void RemoveWorker(string name)
        {
            Worker worker = DB.GetWorker(name);
            if (Exists(this, worker.Id))
                Staff.RemoveAt(Staff.FindIndex(w => w.Id == worker.Id));
        }

        public static void UpdateDB(Shift shift)
        {
            shift.User = DB.GetUser(Session.Current.UserId);
            shift.LastModified = DateTime.Now;
            DB.UpdateShift(shift);
            Logger.Log(shift, MessageType.Update);
        }

        public static void AddToDB(Shift shift)
        {
            shift.User = DB.GetUser(Session.Current.UserId);
            shift.LastModified = DateTime.Now;
            shift.Version++;
            DB.CreateShift(shift);
            Logger.Log(shift, MessageType.Create);
        }

        public static void RemoveFromDB(DateTime date)
        {
            Shift removedShift = DB.GetShift(date);
            DB.RemoveShifts(date);
            Logger.Log(removedShift, MessageType.Delete);
        }

        public static void RemoveFromDB(DateTime date, int version)
        {
            Shift removedShift = DB.RemoveShift(date, version);
            Logger.Log(removedShift, MessageType.Delete);
        }

        private static bool Exists(Shift shift, int id)
        {
            return shift?.Staff?.Exists(w => w.Id == id) ?? false;
        }

        public static bool Validate(Shift shift)
        {
            return shift.Staff.Count > 0;
        }

        public static List<WorkerViewItem> GetWorkerViewItems(Shift shift)
        {
            List<WorkerViewItem> workers = new();
            foreach (Worker worker in DB.GetStaff())
            {
                if (worker.IsActive)
                {
                    WorkerViewItem workerItem = new() { Name = worker.Name };
                    // Поставить галочки работникам, которые были в смене.
                    if (Exists(shift, worker.Id))
                        workerItem.Checked = true;
                    workers.Add(workerItem);
                }
            }
            return workers;
        }

        public static List<ShiftLogItem> GetExcelShiftCollection(DateTime startPeriod, DateTime endPeriod)
        {
            List<Shift> shifts = DB.GetShifts(startPeriod, endPeriod);
            List<ShiftLogItem> collection = new();
            foreach (Shift item in shifts)
                collection.Add(new ShiftLogItem(item));
            return collection;
        }
    }
}