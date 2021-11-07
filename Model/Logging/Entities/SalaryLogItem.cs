using Cashbox.Model.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cashbox.Model.Logging.Entities
{
    public class SalaryLogItem : ILogItem
    {
        public SalaryLogItem(Salary salary)
        {
            Money = salary.Money;
            StartPeriod = Formatter.FormatDate(salary.StartPeriod);
            EndPeriod = Formatter.FormatDate(salary.EndPeriod);
            WorkerName = Worker.GetWorker(salary.WorkerId).Name;
        }

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
    }
}