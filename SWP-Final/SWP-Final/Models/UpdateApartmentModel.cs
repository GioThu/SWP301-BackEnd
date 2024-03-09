public class UpdateApartmentModel
{
    public string ApartmentId { get; set; }
    public int NumberOfBedrooms { get; set; }
    public int NumberOfBathrooms { get; set; }
    public string Furniture { get; set; }
    public decimal Price { get; set; }
    public double Area { get; set; }
    public string Description { get; set; }
    public IFormFile? ApartmentType { get; set; }

}
