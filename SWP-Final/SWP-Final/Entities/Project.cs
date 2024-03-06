using System;
using System.Collections.Generic;

namespace SWP_Final.Entities
{
    public partial class Project
    {
        public Project()
        {
            Buildings = new HashSet<Building>();
            ProjectUtilities = new HashSet<ProjectUtility>();
        }

        public string ProjectId { get; set; } = null!;
        public int? Year { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Building> Buildings { get; set; }
        public virtual ICollection<ProjectUtility> ProjectUtilities { get; set; }
    }
}
