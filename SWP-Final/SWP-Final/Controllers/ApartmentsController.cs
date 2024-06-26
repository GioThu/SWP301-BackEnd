﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_Final.Entities;
using SWP_Final.Models;

namespace SWP_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApartmentsController : ControllerBase
    {
        private readonly RealEasteSWPContext _context;

        public ApartmentsController(RealEasteSWPContext context)
        {
            _context = context;
        }

        // GET: api/Apartments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Apartment>>> GetApartments()
        {
            if (_context.Apartments == null)
            {
                return NotFound();
            }
            var apartmentlist = await _context.Apartments.ToListAsync();

            // Check if the agency list is empty
            if (apartmentlist.Count == 0)
            {
                return NotFound("No agencies found.");
            }

            bool changesMade = false;
            foreach (var apartment in apartmentlist)
            {
                if (apartment.ApartmentType == null || apartment.ApartmentType.Length == 0)
                {
                    apartment.ApartmentType = "Images/common/noimage.png"; // Update with your default image path
                    changesMade = true;
                }
            }

            // Save changes if any agency was updated
            if (changesMade)
            {
                await _context.SaveChangesAsync();

            }
            return await _context.Apartments.ToListAsync();
        }

        // GET: api/Apartments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Apartment>> GetApartment(string id)
        {
            if (_context.Apartments == null)
            {
                return NotFound();
            }
            var apartment = await _context.Apartments.FindAsync(id);

            if (apartment == null)
            {
                return NotFound();
            }

            return apartment;
        }


        // POST: api/Apartments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Apartment>> PostApartment(Apartment apartment)
        {
            if (_context.Apartments == null)
            {
                return Problem("Entity set 'RealEasteSWPContext.Apartments'  is null.");
            }
            _context.Apartments.Add(apartment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ApartmentExists(apartment.ApartmentId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetApartment", new { id = apartment.ApartmentId }, apartment);
        }

        // DELETE: api/Apartments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApartment(string id)
        {
            if (_context.Apartments == null)
            {
                return NotFound();
            }
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }

            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("FilterByBedrooms")]
        public async Task<ActionResult<IEnumerable<Apartment>>> FilterApartmentsByBedrooms([FromBody] int bedrooms)
        {
            var filteredApartments = await _context.Apartments
                                                .Where(a => a.NumberOfBedrooms == bedrooms)
                                                .ToListAsync();

            if (filteredApartments.Count == 0)
            {
                return NotFound($"No apartments found with {bedrooms} bedroom(s).");
            }

            return filteredApartments;
        }

        // POST: api/Apartments/FilterByBathrooms
        [HttpPost("FilterByBathrooms")]
        public async Task<ActionResult<IEnumerable<Apartment>>> FilterApartmentsByBathrooms([FromBody] int bathrooms)
        {
            var filteredApartments = await _context.Apartments
                                                .Where(a => a.NumberOfBathrooms == bathrooms)
                                                .ToListAsync();

            if (filteredApartments.Count == 0)
            {
                return NotFound($"No apartments found with {bathrooms} bathroom(s).");
            }

            return filteredApartments;
        }

        // POST: api/Apartments/FilterByPriceRange
        [HttpPost("FilterByPriceRange")]
        public async Task<ActionResult<IEnumerable<Apartment>>> FilterApartmentsByPriceRange([FromBody] PriceFilterModel filter)
        {
            var filteredApartments = await _context.Apartments
                                                .Where(a => a.Price >= filter.MinPrice && a.Price <= filter.MaxPrice)
                                                .ToListAsync();

            if (filteredApartments.Count == 0)
            {
                return NotFound($"No apartments found within the price range ${filter.MinPrice} to ${filter.MaxPrice}.");
            }

            return filteredApartments;
        }

        [HttpGet("ListApartmentByAgency")]
        public async Task<ActionResult<IEnumerable<Apartment>>> ListApartmentsByAgency(string agencyId)
        {
            // Retrieve apartments associated with the specified agency
            var apartmentsByAgency = await _context.Apartments
                                                    .Where(a => a.AgencyId == agencyId)
                                                    .ToListAsync();

            if (apartmentsByAgency.Count == 0)
            {
                return NotFound($"No apartments found for agency with ID: {agencyId}");
            }

            return apartmentsByAgency;
        }

        [HttpPut("UpdateApartment")]
        public async Task<IActionResult> UpdateApartment([FromForm] UpdateApartmentModel apartment)
        {
            var existingApartment = await _context.Apartments.FindAsync(apartment.ApartmentId);
            if (existingApartment == null)
            {
                return NotFound();
            }

            if (apartment.ApartmentType != null && apartment.ApartmentType.Length > 0)
            {
                string filename = "Images/AsImage/" + apartment.ApartmentType.FileName;

                var path = GetFilePath(filename);
                using (var stream = System.IO.File.Create(path))
                {
                    await apartment.ApartmentType.CopyToAsync(stream);
                }
                existingApartment.ApartmentType = filename;
            }

            // Update the existing apartment's properties
            existingApartment.NumberOfBedrooms = apartment.NumberOfBedrooms;
            existingApartment.NumberOfBathrooms = apartment.NumberOfBathrooms;
            existingApartment.Furniture = apartment.Furniture;
            existingApartment.Price = apartment.Price;
            existingApartment.Area = apartment.Area;
            existingApartment.Description = apartment.Description;
            existingApartment.Status = "Waiting";


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApartmentExists(apartment.ApartmentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //GET: api/Agencies/GetImage/
        [HttpGet("GetApartmentImage/{id}")]
        public async Task<IActionResult> GetApartmentImage(string id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound("The object does not exist or has been deleted.");
            }
            string filename = "";
            if (apartment.ApartmentType == null || apartment.ApartmentType.Length == 0)
            {
                filename = "Images/common/noimage.png";
            }
            else
            {
                filename = apartment.ApartmentType;
            }

            var path = GetFilePath(filename);
            if (!System.IO.File.Exists(path))
            {
                return NotFound("File does not exist.");
            }

            var imageStream = System.IO.File.OpenRead(path);


            var mimeType = "image/jpeg";
            if (Path.GetExtension(path).ToLower() == ".png")
            {
                mimeType = "image/png";
            }
            else if (Path.GetExtension(path).ToLower() == ".gif")
            {
                mimeType = "image/gif";
            }
            return File(imageStream, mimeType);
        }

        [HttpGet("GetApartmentsByBuildingID")]
        public async Task<ActionResult<IEnumerable<Apartment>>> GetApartmentsByBuildingID(string buildingId)
        {
            // Lấy danh sách các căn hộ thuộc tòa nhà có buildingId tương ứng và sắp xếp theo diện tích từ thấp đến cao
            var apartmentsByBuilding = await _context.Apartments
                                                    .Where(a => a.BuildingId == buildingId)
                                                    .OrderBy(a => a.ApartmentId)
                                                    .ToListAsync();

            if (apartmentsByBuilding.Count == 0)
            {
                return NotFound($"Không tìm thấy căn hộ nào cho tòa nhà có ID: {buildingId}");
            }

            return apartmentsByBuilding;
        }

        [HttpGet("GetApartmentsByBuildingIDForBooking")]
        public async Task<ActionResult<IEnumerable<Apartment>>> GetApartmentsByBuildingIDForBooking(string buildingId)
        {
            // Lấy danh sách các căn hộ thuộc tòa nhà có buildingId tương ứng và sắp xếp theo diện tích từ thấp đến cao,
            // chỉ lấy những căn hộ có trạng thái là "Updated"
            var apartmentsByBuilding = await _context.Apartments
                                                    .Where(a => a.BuildingId == buildingId && a.Status == "Updated")
                                                    .OrderBy(a => a.ApartmentId)
                                                    .ToListAsync();

            if (apartmentsByBuilding.Count == 0)
            {
                return NotFound($"Không tìm thấy căn hộ nào cho tòa nhà có ID: {buildingId} và trạng thái là 'Updated'");
            }

            return apartmentsByBuilding;
        }

        [HttpGet("GetBookingByApartmentId/{apartmentId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingByApartmentId(string apartmentId)
        {
            // Query the database for bookings related to the specified apartment id that are active
            var activeBookings = await _context.Bookings
                                                .Where(b => b.ApartmentId == apartmentId && b.Status == "Active")
                                                .ToListAsync();

            if (activeBookings.Count == 0)
            {
                return NotFound($"No active bookings found for apartment with ID: {apartmentId}");
            }

            return activeBookings;
        }
        [HttpGet("GetListBuildingIdByAgencyId/{id}")]
        public async Task<ActionResult<IEnumerable<string>>> GetListBuildingIdByAgencyId(string id)
        {
            if (_context.Apartments == null)
            {
                return NotFound();
            }

            // Lấy danh sách duy nhất các BuildingID dựa trên AgencyId
            var buildingIds = await _context.Apartments
                                            .Where(a => a.AgencyId == id)
                                            .Select(a => a.BuildingId) // Chọn ra trường BuildingId
                                            .Distinct() // Loại bỏ các giá trị trùng lặp
                                            .ToListAsync();

            if (buildingIds == null || buildingIds.Count == 0)
            {
                return NotFound();
            }

            return buildingIds; // Trả về danh sách các BuildingID duy nhất
        }


        [HttpGet("GetApartmentsByAgencyIdAndBuildingId/{agencyId}/{buildingId}")]
        public async Task<ActionResult<IEnumerable<Apartment>>> GetApartmentsByAgencyIdAndBuildingId(string agencyId, string buildingId)
        {
            // Lấy danh sách các căn hộ thuộc tòa nhà có buildingId và agencyId tương ứng,
            // loại trừ các căn hộ có trạng thái là "Sold", và sắp xếp theo diện tích từ thấp đến cao
            var apartmentsByBuildingAndAgency = await _context.Apartments
                                                        .Where(a => a.BuildingId == buildingId && a.AgencyId == agencyId)
                                                        .OrderBy(a => a.ApartmentId)
                                                        .ToListAsync();

            if (apartmentsByBuildingAndAgency.Count == 0)
            {
                return NotFound(new { Message = $"Không tìm thấy căn hộ nào phù hợp cho tòa nhà có ID: {buildingId} và đại lý có ID: {agencyId} hoặc tất cả đều đã được bán." });
            }

            return apartmentsByBuildingAndAgency;
        }


        //POST: api/Apartment/UploadInfoWithImage


        [HttpPost("UploadInformationWithImage/{id}")]
        public async Task<IActionResult> UploadInformationWithImage([FromForm] ApartmentModel apartmentModel, string id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound("Apartment not found");
            }

            // Update non-image properties regardless of whether an image is provided
            apartment.Description = apartmentModel.Description;
            apartment.NumberOfBathrooms = apartmentModel.NumberOfBathrooms;
            apartment.NumberOfBedrooms = apartmentModel.NumberOfBedrooms;
            apartment.Price = apartmentModel.Price;
            apartment.Furniture = apartmentModel.Furniture;
            apartment.Area = apartmentModel.Area;
            apartment.Status = "Waiting";

            // Process image only if provided
            if (apartmentModel.ApartmentType != null && apartmentModel.ApartmentType.Length > 0)
            {
                string fileNameImageApartmentModel = $"Images/ApartmentsImage/{Path.GetFileName(apartmentModel.ApartmentType.FileName)}";
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileNameImageApartmentModel);

                // Ensure the directory exists
                var directoryName = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                // Save new file
                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    await apartmentModel.ApartmentType.CopyToAsync(stream);
                }

                int count = _context.Apartments.Count(a => a.ApartmentType == apartment.ApartmentType && a.ApartmentId != apartment.ApartmentId); // Ensure not counting the current apartment

                // Delete the old image if it is different from the new one and it's not the default image
                if (apartment.ApartmentType != fileNameImageApartmentModel && apartment.ApartmentType != valiablenoimage() && count == 0)
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", apartment.ApartmentType);
                    if (System.IO.File.Exists(oldImagePath) && apartment.ApartmentType != valiablenoimage())
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                apartment.ApartmentType = fileNameImageApartmentModel;
            }


            await _context.SaveChangesAsync();
            return Ok(apartment);
        }


        [HttpGet("GetAllAgencyAndNumberOfApartment")]
        public async Task<ActionResult<IEnumerable<AgencyApartmentCountModel>>> GetAllAgencyAndNumberOfApartment()
        {
            var agenciesWithApartmentCounts = await _context.Agencies
             .Select(agency => new
             {
                 Agency = agency,
                 ApartmentCount = _context.Apartments .Where(apartment => apartment.AgencyId == agency.AgencyId && apartment.Status != "Sold")
                                                                        .Count()
             })
                                                .Select(result => new AgencyApartmentCountModel
                                                {
                                                    AgencyId = result.Agency.AgencyId,
                                                    AgencyFirstName = result.Agency.FirstName,
                                                    AgencyLastName = result.Agency.LastName,
                                                    NumberOfApartments = result.ApartmentCount

                                                })
                                                .ToListAsync();

            return agenciesWithApartmentCounts;
        }


        [HttpGet("GetListBuildingIdByApartmentId/{apartmentId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetListBuildingIdByApartmentId(string apartmentId)
        {
            if (_context.Apartments == null)
            {
                return NotFound();
            }

            // Lấy danh sách duy nhất các BuildingID dựa trên AgencyId
            var buildingIds = await _context.Apartments
                                            .Where(a => a.ApartmentId == apartmentId)
                                            .Select(a => a.BuildingId) // Chọn ra trường BuildingId
                                            .Distinct() // Loại bỏ các giá trị trùng lặp
                                            .ToListAsync();

            if (buildingIds == null || buildingIds.Count == 0)
            {
                return NotFound();
            }

            return buildingIds; // Trả về danh sách các BuildingID duy nhất
        }


        [HttpGet("GetWaitingApartments")]
        public async Task<ActionResult<IEnumerable<Apartment>>> GetWaitingApartments()
        {
            // Retrieve apartments with status "Waiting"
            var waitingApartments = await _context.Apartments
                                                .Where(a => a.Status == "Waiting")
                                                .ToListAsync();

            if (waitingApartments.Count == 0)
            {
                return NotFound("No apartments found with status 'Waiting'.");
            }

            return waitingApartments;
        }

        [HttpPut("ChangeApartmentStatus/{id}")]
        public async Task<IActionResult> ChangeApartmentStatus(string id, string newStatus)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound("Apartment not found");
            }

            // Update the apartment's status
            apartment.Status = newStatus;

            if (newStatus == "Distributed") 
            {
                apartment.ApartmentType = "Images/common/noimage.png";
                apartment.Description = null;
                apartment.NumberOfBathrooms = 0;
                apartment.NumberOfBedrooms = 0;
                apartment.Furniture = null;
                apartment.Area = null;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok($"Apartment status changed to '{newStatus}' successfully.");
            }
            catch (DbUpdateException)
            {
                // Log the error or handle it appropriately
                return StatusCode(500, "An error occurred while changing apartment status.");
            }
        }

        [HttpGet("GetRoomNumberByApartmentId/{apartmentId}")]
        public async Task<ActionResult<string>> GetRoomNumberByApartmentId(string apartmentId)
        {
            // Kiểm tra apartmentId có đúng định dạng buildingId:xxx không
            if (!apartmentId.Contains(":"))
            {
                return BadRequest("Invalid apartmentId format. It should be in the format 'buildingId:xxx'.");
            }

            // Tách buildingId từ apartmentId
            var buildingId = apartmentId.Split(':')[0];

            // Tìm building dựa trên buildingId
            var building = await _context.Buildings.FindAsync(buildingId);
            if (building == null)
            {
                return NotFound("Building not found.");
            }

            // Lấy số phòng từ apartmentId
            var roomNumber = apartmentId.Split(':')[1];

            // Kết hợp tên của building và số phòng để tạo chuỗi kết quả
            var result = $"{building.Name} - Room {roomNumber}";

            return result;
        }

        [NonAction]

        private string valiablenoimage() => "Images/common/noimage.png";

        private string GetFilePath(string filename) => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);

        private bool ApartmentExists(string id)
        {
            return (_context.Apartments?.Any(e => e.ApartmentId == id)).GetValueOrDefault();
        }
    }
}
