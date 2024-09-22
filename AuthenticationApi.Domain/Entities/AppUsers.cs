
namespace AuthenticationApi.Domain.Entities
{
    public class AppUsers
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phonenumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
