namespace PBS.DSS.Shared.Models
{
    public class Contact
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}