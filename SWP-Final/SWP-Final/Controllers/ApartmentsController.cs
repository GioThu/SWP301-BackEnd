using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            existingApartment.Status = "Updated";


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
            if (apartment == null )
            {
                return NotFound("The image does not exist or has been deleted.");
            }

            var path = GetFilePath(apartment.ApartmentType);
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
                                                    .OrderBy(a => a.Area)
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
                                                    .OrderBy(a => a.Area)
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



        private string valiablenoimage() => "Images/common/noimage.png";

        private string GetFilePath(string filename) => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);

        private bool ApartmentExists(string id)
        {
            return (_context.Apartments?.Any(e => e.ApartmentId == id)).GetValueOrDefault();
        }
    }
}
