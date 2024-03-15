using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_Final.Entities;

namespace SWP_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly RealEasteSWPContext _context;

        public BookingsController(RealEasteSWPContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            if (_context.Bookings == null)
            {
                return NotFound();
            }
            return await _context.Bookings.ToListAsync();
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(string id)
        {
            if (_context.Bookings == null)
            {
                return NotFound();
            }
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }


        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            if (_context.Bookings == null)
            {
                return NotFound();
            }
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("GetAllBookingByApartmentID/{apartmentId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookingByApartmentID(string apartmentId)
        {
            var bookings = await _context.Bookings
                                        .Where(b => b.ApartmentId == apartmentId && (b.Status == "Active" || b.Status == "Complete"))
                                        .ToListAsync();

            if (bookings == null || bookings.Count == 0)
            {
                return NotFound("No bookings found for the specified apartment.");
            }

            return bookings;
        }

        [HttpPut("CompleteBooking/{id}")]
        public async Task<IActionResult> CompleteBooking(string id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Kiểm tra xem trạng thái hiện tại có phải là "Active" hay không
            if (booking.Status != "Active")
            {
                return BadRequest("The booking is not currently active.");
            }

            // Cập nhật trạng thái thành "Complete"
            booking.Status = "Complete";

            // Lưu các thay đổi vào cơ sở dữ liệu
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(booking.BookingId))
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


        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(string customerId, string apartmentId)
        {
            if (BookingExistsForApartment(customerId, apartmentId))
            {
                return Conflict(new { massage = "A booking with the same Customer and Apartment already exists." });
            }

            // Fetch AgencyId associated with the Apartment
            var apartment = await _context.Apartments.FindAsync(apartmentId);
            if (apartment == null)
            {
                return NotFound("Apartment not found.");
            }
            string agencyId = apartment.AgencyId;

            // Create a new Booking object with automatically generated BookingId
            var booking = new Booking
            {
                CustomerId = customerId,
                ApartmentId = apartmentId,
                AgencyId = agencyId,
                Status = "Active", // Set status to "Active"
                BookingId = Guid.NewGuid().ToString(), // Generate a unique BookingID
                Date = DateTime.Now
            };

            _context.Bookings.Add(booking);
            try
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve // Preserve reference to avoid object cycles
                };
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookingExists(booking.BookingId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
        }


        [HttpGet("GetAllBookingsByCustomerId/{customerId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookingsByCustomerId(string customerId)
        {
            var bookingbycustomer = await _context.Bookings
                                                   .Where(booking=>booking.CustomerId == customerId)
                                                   .OrderBy(booking => booking.BookingId)
                                                   .ToListAsync();
            if (bookingbycustomer == null || bookingbycustomer.Count==0)
            {
                return NotFound();
            }
            return bookingbycustomer;
        }

        [NonAction]
        // Method to check if a booking with the same CustomerId and ApartmentId already exists
        private bool BookingExistsForApartment(string customerId, string apartmentId)
        {
            return _context.Bookings.Any(b => b.CustomerId == customerId && b.ApartmentId == apartmentId);
        }

        // Method to check if a booking with the given BookingId already exists
        private bool BookingExists(string id)
        {
            return (_context.Bookings?.Any(e => e.BookingId == id)).GetValueOrDefault();
        }

    }
}
