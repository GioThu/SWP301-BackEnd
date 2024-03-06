using System;
using System.Collections.Generic;

namespace SWP_Final.Entities
{
    public partial class Booking
    {
        public string BookingId { get; set; } = null!;
        public DateTime? Date { get; set; }
        public string? AgencyId { get; set; }
        public string? ApartmentId { get; set; }
        public string? CustomerId { get; set; }
        public string? Status { get; set; }

        public virtual Agency? Agency { get; set; }
        public virtual Apartment? Apartment { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
