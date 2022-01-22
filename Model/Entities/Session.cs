using Cashbox.Model.Logging;
using Cashbox.Model.Logging.Entities;
using Cashbox.Model.Repositories;

namespace Cashbox.Model.Entities
{
    public class Session : ILogged
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public static Session Current { get; private set; }

        public ILogItem ConvertToLogItem() => (SessionLogItem)this;

        public static void InitSession(string userName)
        {
            Current = SessionRepo.CreateSession(userName);
            Logger.Log(Current, MessageType.Create);
        }

        public static void RemoveCurrentSession()
        {
            SessionRepo.RemoveSession(Current.Id);
            Logger.Log(Current, MessageType.Delete);
        }

        public static Permissions GetPermissions()
        {
            return Permissions.GetAccesses(Current.UserId);
        }
    }
}