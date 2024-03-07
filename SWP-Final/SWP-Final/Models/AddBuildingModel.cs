namespace SWP_Final.Models
{
    public class AddBuildingModel
    {
        public string ProjectId { get; set; }
        public int NumberOfFloor { get; set; }
        public int NumberOfApartment { get; set; }
        public string Description { get; set; }
        public IFormFile? FileImage { get; set; }
        public string Name { get; set; }
    }
}
