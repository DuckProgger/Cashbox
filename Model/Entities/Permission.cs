using Cashbox.Model.Repositories;

namespace Cashbox.Model.Entities
{
    public class Permissions
    {
        public int Id { get; set; }
        public bool IsAdmin { get; set; }

        public User User { get; set; }

        public static Permissions GetAccesses(int userId)
        {
            var user = UserRepo.GetUser(userId);
            return user.Permissions;
        }
    }
}