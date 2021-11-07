using Cashbox.Model.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cashbox.Model.Logging.Entities
{
    public class SalaryLogItem : ILogItem
    {
        [Description("Сумма")]
        public int Money { get; set; }

        [Description("С")]
        public string StartPeriod { get; set; }

        [Description("По")]
        public string EndPeriod { get; set; }

        [Description("Работник")]
        public string WorkerName { get; set; }

        public int GetMessageId(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.Create => 10,
                MessageType.Update => 11,
                MessageType.Delete => 12,
                _ => 0
            };
        }

        public Dictionary<string, object> GetPropertiesInfo()
        {
            return Util.GetPropertiesInfo(this);
        }

        public static implicit operator SalaryLogItem(Salary entity)
        {
            return new SalaryLogItem()
            {
                Money = entity.Money,
                StartPeriod = Formatter.FormatDate(entity.StartPeriod),
                EndPeriod = Formatter.FormatDate(entity.EndPeriod),
                WorkerName = Worker.GetWorker(entity.WorkerId).Name
            };
        }
    }
}