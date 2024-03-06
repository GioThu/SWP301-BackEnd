using System;
using System.Collections.Generic;

namespace SWP_Final.Entities
{
    public partial class Post
    {
        public string PostId { get; set; } = null!;
        public DateTime? SalesOpeningDate { get; set; }
        public DateTime? SalesClosingDate { get; set; }
        public DateTime? PostDate { get; set; }
        public string? Images { get; set; }
        public string? Description { get; set; }
        public string? PriorityMethod { get; set; }
        public string? BuildingId { get; set; }
        public string? AgencyId { get; set; }

        public virtual Agency? Agency { get; set; }
        public virtual Building? Building { get; set; }
    }
}
