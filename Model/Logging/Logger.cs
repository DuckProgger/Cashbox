using Cashbox.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using Cashbox.Model.Entities;
using Cashbox.Model.Logging.Entities;

namespace Cashbox.Model.Logging
{
    public enum MessageType { Create, Update, Delete }

    public class Logger
    {
        public static void Log(IEntity entity, MessageType type)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"{DateTime.Now}: ");

            ILogItem logItem = GetLogItem(entity);
            Dictionary<string, object> propsInfo = null;

            switch (type)
            {
                case MessageType.Create:
                    switch (logItem)
                    {
                        case ShiftLogItem item:
                            stringBuilder.Append(XmlService.GetMessageText(1));
                            propsInfo = Util.GetPropertiesInfo(item);
                            break;

                        case SessionLogItem item:
                            stringBuilder.Append(XmlService.GetMessageText(2));
                            propsInfo = Util.GetPropertiesInfo(item);
                            break;

                        default:
                            break;
                    }
                    break;

                case MessageType.Update:
                    break;

                case MessageType.Delete:
                    break;
            }

            foreach (var prop in propsInfo)
                stringBuilder.Append($"{prop.Key} = {prop.Value} ");
            stringBuilder.Append('\n');
            File.AppendAllText("log.txt", stringBuilder.ToString());
        }

        private static ILogItem GetLogItem(IEntity entity)
        {
            return entity switch
            {
                Shift shift => new ShiftLogItem(shift),
                Session session => new SessionLogItem(session),
                _ => throw new NotImplementedException(),
            };
        }
    }
}