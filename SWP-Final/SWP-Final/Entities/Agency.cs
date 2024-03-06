using System;
using System.Collections.Generic;

namespace SWP_Final.Entities
{
    public partial class Agency
    {
        public Agency()
        {
            Apartments = new HashSet<Apartment>();
            Bookings = new HashSet<Booking>();
            Orders = new HashSet<Order>();
            Posts = new HashSet<Post>();
        }

        public string AgencyId { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Images { get; set; }
        public string? UserId { get; set; }
        public string? Phone { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Apartment> Apartments { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
