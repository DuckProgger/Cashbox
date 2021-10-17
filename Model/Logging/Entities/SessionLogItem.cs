﻿using Cashbox.Model.Entities;
using Cashbox.Model.Managers;
using System.Collections.Generic;
using System.ComponentModel;

namespace Cashbox.Model.Logging.Entities
{
    public class SessionLogItem : ILogItem
    {
        public SessionLogItem(Session entity)
        {
            UserName = UserManager.GetUser(entity.UserId).Name;
        }

        [Description("Пользователь")]
        public string UserName { get; set; }

        public int GetMessageId(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.Create => 4,
                MessageType.Update => 5,
                MessageType.Delete => 6,
                _ => 0
            };
        }

        public Dictionary<string, object> GetPropertiesInfo()
        {
            return Util.GetPropertiesInfo(this);
        }
    }
}