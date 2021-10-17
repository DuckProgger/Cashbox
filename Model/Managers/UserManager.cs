using Cashbox.Exceptions;
using Cashbox.Model.Entities;

namespace Cashbox.Model.Managers
{
    public class UserManager
    {
        public static User GetUser(int id)
        {
            return DB.GetUser(id) ?? throw new InvalidNameException("Пользователь не найден");
        }
    }
}