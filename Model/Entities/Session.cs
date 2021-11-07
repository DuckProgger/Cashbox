using Cashbox.Model.Logging;
using Cashbox.Model.Logging.Entities;

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
            Current = DB.CreateSession(userName);
            Logger.Log(Current, MessageType.Create);
        }

        public static void RemoveCurrentSession()
        {
            DB.RemoveSession(Current.Id);
            Logger.Log(Current, MessageType.Delete);
        }

        public static Permissions GetPermissions()
        {
            return Permissions.GetAccesses(Current.UserId);
        }
    }
}