using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SWP_Final.Models
{
    public class AgencyRegisterModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile FileImage { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
