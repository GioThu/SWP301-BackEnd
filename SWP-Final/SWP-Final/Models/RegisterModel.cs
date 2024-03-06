using Microsoft.AspNetCore.Http;

namespace SWP_Final.Models
{
    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public IFormFile? FileImage { get; set; }
    }
}
