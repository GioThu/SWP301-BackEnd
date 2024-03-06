using System;

namespace SWP_Final.Models
{
    public class BookingModel
    {
        public string BookingId { get; set; }
        public DateTime? Date { get; set; }
        public string AgencyId { get; set; }
        public string ApartmentId { get; set; }
        public string CustomerId { get; set; }
        public string Status { get; set; }
    }
}
