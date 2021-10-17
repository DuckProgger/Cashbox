using Cashbox.Model.Entities;
using Cashbox.Model.Logging;

namespace Cashbox.Model.Managers
{
    public class SessionManager
    {
        public static Session Session { get; private set; }

        public static void InitSession(string userName)
        {
            Session = DB.CreateSession(userName);
            Logger.Log(Session, MessageType.Create);
        }

        public static void RemoveCurrentSession()
        {
            DB.RemoveSession(Session.Id);
            Logger.Log(Session, MessageType.Delete);
        }

        public static Permissions GetPermissions()
        {
            return Permissions.GetAccesses(Session.UserId);
        }
    }
}