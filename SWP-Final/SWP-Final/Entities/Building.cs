using System;
using System.Collections.Generic;

namespace SWP_Final.Entities
{
    public partial class Building
    {
        public Building()
        {
            Apartments = new HashSet<Apartment>();
            Posts = new HashSet<Post>();
        }

        public string BuildingId { get; set; } = null!;
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ProjectId { get; set; }
        public string? TypeOfRealEstate { get; set; }
        public int? NumberOfFloors { get; set; }
        public int? NumberOfApartments { get; set; }
        public string? Status { get; set; }
        public DateTime? YearOfConstruction { get; set; }
        public string? Images { get; set; }
        public string? Describe { get; set; }
        public string? Investor { get; set; }
        public double? Area { get; set; }
        public string? Amenities { get; set; }

        public virtual Project? Project { get; set; }
        public virtual ICollection<Apartment> Apartments { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
