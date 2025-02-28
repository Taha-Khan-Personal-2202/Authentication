namespace Authentication.Shared.Model
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; }
        public string? UserId { get; set; } = string.Empty;
        public string? NewPassword { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? token {get;set;} = string.Empty;

        public List<string>? Permissions { get; set; }
    }
}
