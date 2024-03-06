using System;
using System.Collections.Generic;

namespace SWP_Final.Entities
{
    public partial class Apartment
    {
        public Apartment()
        {
            Bookings = new HashSet<Booking>();
            Orders = new HashSet<Order>();
        }

        public string ApartmentId { get; set; } = null!;
        public string? BuildingId { get; set; }
        public int? NumberOfBedrooms { get; set; }
        public int? NumberOfBathrooms { get; set; }
        public string? Furniture { get; set; }
        public double? Area { get; set; }
        public decimal? Price { get; set; }
        public string? AgencyId { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? ApartmentType { get; set; }
        public int? FloorNumber { get; set; }

        public virtual Agency? Agency { get; set; }
        public virtual Building? Building { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
