using System;
using System.Collections.Generic;

namespace SWP_Final.Entities
{
    public partial class ProjectUtility
    {
        public string ProjectUtilitiesId { get; set; } = null!;
        public string? ProjectId { get; set; }
        public string? UtilitiesId { get; set; }

        public virtual Project? Project { get; set; }
        public virtual Utility? Utilities { get; set; }
    }
}
