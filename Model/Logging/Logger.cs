using Cashbox.Model.Entities;
using Cashbox.Model.Logging.Entities;
using Cashbox.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cashbox.Model.Logging
{
    public enum MessageType { Create, Update, Delete }

    public class Logger
    {
        public static void Log(IEntity entity, MessageType type)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"{DateTime.Now}: ");

            ILogItem logItem = entity.ConvertToLogItem();

            int messageId = logItem.GetMessageId(type);
            string messageText = XmlService.GetMessageText(messageId);
            stringBuilder.Append(messageText);

            foreach (KeyValuePair<string, object> prop in logItem.GetPropertiesInfo())
                stringBuilder.Append($"{prop.Key} = {prop.Value} ");
            stringBuilder.Append("\n\n");
            File.AppendAllText("log.txt", stringBuilder.ToString());
        }
    }
}