using Cashbox.Model.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cashbox.Model.Logging.Entities
{
    public class WorkerLogItem : ILogItem
    {
        public WorkerLogItem(Worker worker)
        {
            Name = worker.Name;
        }

        [Description("Имя")]
        public string Name { get; set; }

        public int GetMessageId(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.Create => 7,
                MessageType.Update => 8,
                MessageType.Delete => 9,
                _ => 0
            };
        }

        public Dictionary<string, object> GetPropertiesInfo()
        {
            return Util.GetPropertiesInfo(this);
        }
    }
}