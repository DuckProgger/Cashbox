using Cashbox.Model.Logging.Entities;

namespace Cashbox.Model.Entities
{
    public interface ILogged
    {
        public ILogItem ConvertToLogItem(/*IEntity entity*/);
    }
}
