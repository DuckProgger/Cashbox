namespace Cashbox.Model.Entities
{
    public class Permissions : IEntity
    {
        public int Id { get; set; }
        public bool IsAdmin { get; set; }

        public User User { get; set; }

        public static Permissions GetAccesses(int userId)
        {
            var user = DB.GetUser(userId);
            return user.Permissions;
        }
    }
}