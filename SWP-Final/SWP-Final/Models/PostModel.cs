using System;

namespace SWP_Final.Models
{
    public class PostModel
    {
        public string PostId { get; set; }
        public DateTime? SalesOpeningDate { get; set; }
        public DateTime? SalesClosingDate { get; set; }
        public DateTime? PostDate { get; set; }
        public string Images { get; set; }
        public string Description { get; set; }
        public string PriorityMethod { get; set; }
        public string BuildingId { get; set; }
        public string AgencyId { get; set; }
    }
}
