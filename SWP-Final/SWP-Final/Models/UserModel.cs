namespace SWP_Final.Models
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RoleId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
