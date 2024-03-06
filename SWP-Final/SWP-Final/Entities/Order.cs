using System;
using System.Collections.Generic;

namespace SWP_Final.Entities
{
    public partial class Order
    {
        public string OrderId { get; set; } = null!;
        public DateTime? Date { get; set; }
        public string? AgencyId { get; set; }
        public string? ApartmentId { get; set; }
        public string? Status { get; set; }
        public decimal? TotalAmount { get; set; }

        public virtual Agency? Agency { get; set; }
        public virtual Apartment? Apartment { get; set; }
    }
}
