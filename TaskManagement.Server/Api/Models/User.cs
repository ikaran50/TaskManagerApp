namespace TaskManagement.Server.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAtDate { get; set; } = DateTime.UtcNow;
    }
}