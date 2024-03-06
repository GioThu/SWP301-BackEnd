using System;
using System.Collections.Generic;

namespace SWP_Final.Entities
{
    public partial class User
    {

        public User()
        {
            UserId = Guid.NewGuid().ToString(); // Tạo một GUID và chuyển nó thành một chuỗi
            Agencies = new HashSet<Agency>();
            Customers = new HashSet<Customer>();
        }

        public string UserId { get; set; } = null!;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? RoleId { get; set; }
        public string? Status { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual ICollection<Agency> Agencies { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
