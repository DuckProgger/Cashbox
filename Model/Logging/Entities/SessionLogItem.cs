using Cashbox.Model.Entities;
using Cashbox.Model.Managers;
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
    }
}