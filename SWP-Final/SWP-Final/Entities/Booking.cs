using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

        public decimal? Money { get; set; }
        public virtual Agency? Agency { get; set; }

        [JsonIgnore]
        public virtual Apartment? Apartment { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
