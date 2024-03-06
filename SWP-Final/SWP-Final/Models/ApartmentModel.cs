namespace SWP_Final.Models
{
    public class ApartmentModel
    {
        public string ApartmentId { get; set; }
        public string BuildingId { get; set; }
        public int NumberOfBedrooms { get; set; }
        public int NumberOfBathrooms { get; set; }
        public string Furniture { get; set; }
        public double Area { get; set; }
        public decimal Price { get; set; }
        public string? AgencyId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string ApartmentType { get; set; }
        public int FloorNumber { get; set; }
    }
}
