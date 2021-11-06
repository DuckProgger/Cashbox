using Cashbox.Model.Logging.Entities;

namespace Cashbox.Model.Entities
{
    public class Session : ILogged
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public ILogItem ConvertToLogItem()
        {
            return new SessionLogItem(this);
        }
    }
}