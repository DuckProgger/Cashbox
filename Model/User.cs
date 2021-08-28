namespace Cashbox.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public Accesses Access { get; set; }

        public enum Accesses { Usual, Administrator }
    }
}
