namespace Cashbox.Model.Entities
{
    public class Session : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }
}