using System;
using System.Collections.Generic;

namespace SWP_Final.Models
{
    public class BuildingModel
    {
        public string? BuildingId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ProjectId { get; set; }
        public string? TypeOfRealEstate { get; set; }
        public int? NumberOfFloors { get; set; }
        public int? NumberOfApartments { get; set; }
        public string? Status { get; set; }
        public DateTime? YearOfConstruction { get; set; }
        public IFormFile? FileImage { get; set; }
        public string? Describe { get; set; }
        public string? Investor { get; set; }
        public double? Area { get; set; }
        public string? Amenities { get; set; }
    }
}