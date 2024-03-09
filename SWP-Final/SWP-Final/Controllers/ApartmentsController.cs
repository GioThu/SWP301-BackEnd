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

        // PUT: api/Apartments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApartment(string id, Apartment apartment)
        {
            if (id != apartment.ApartmentId)
            {
                return BadRequest();
            }

            _context.Entry(apartment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApartmentExists(id))
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

        private string valiablenoimage() => "Images/common/noimage.png";

        private string GetFilePath(string filename) => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);

        private bool ApartmentExists(string id)
        {
            return (_context.Apartments?.Any(e => e.ApartmentId == id)).GetValueOrDefault();
        }
    }
}
