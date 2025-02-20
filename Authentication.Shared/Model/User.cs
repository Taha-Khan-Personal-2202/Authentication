using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.Shared.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [NotMapped]
        public string? UserId { get; set; } = string.Empty; 
    }
}
