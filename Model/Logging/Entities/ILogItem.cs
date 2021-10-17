using Cashbox.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Model.Logging.Entities
{
    public interface ILogItem
    {
        public int GetMessageId(MessageType messageType);

        public Dictionary<string, object> GetPropertiesInfo();
    }
}