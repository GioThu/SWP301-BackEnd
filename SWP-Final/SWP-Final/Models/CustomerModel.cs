using System;
using System.Collections.Generic;

namespace SWP_Final.Models
{
    public class CustomerModel
    {
        public string? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public IFormFile? FileImage { get; set; }
        public string? UserId { get; set; }
        public string? Phone { get; set; }
    }
}