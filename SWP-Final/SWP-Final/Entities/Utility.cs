using System;
using System.Collections.Generic;

namespace SWP_Final.Entities
{
    public partial class Utility
    {
        public Utility()
        {
            ProjectUtilities = new HashSet<ProjectUtility>();
        }

        public string UtilitiesId { get; set; } = null!;
        public string? Name { get; set; }

        public virtual ICollection<ProjectUtility> ProjectUtilities { get; set; }
    }
}
