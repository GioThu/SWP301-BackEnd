using System;

namespace SWP_Final.Models
{
    public class OrderModel
    {
        public string OrderId { get; set; }
        public DateTime? Date { get; set; }
        public string AgencyId { get; set; }
        public string ApartmentId { get; set; }
        public string Status { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
