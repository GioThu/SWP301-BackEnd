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
using SWP_Final.Models;

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
        [HttpDelete("DeleteBooking/{id}")]
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


        [HttpPost("/{customerId}/{apartmentId}/{money}")]
        public async Task<ActionResult<Booking>> PostBooking([FromForm] BookingModel bookingModel ,string customerId, string apartmentId, decimal money)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(apartmentId) || money <= 0 || bookingModel == null)
            {
                return BadRequest("Invalid data provided.");
            }

            if (BookingExistsForApartment(customerId, apartmentId))
            {
                return Conflict(new { message = "A booking with the same Customer and Apartment already exists." });
            }

            var apartment = await _context.Apartments.FindAsync(apartmentId);
            if (apartment == null)
            {
                return NotFound("Apartment not found.");
            }

            var agencyId = apartment.AgencyId;
            var booking = new Booking
            {
                CustomerId = customerId,
                ApartmentId = apartmentId,
                AgencyId = agencyId,
                Status = "Waiting", // Set status to "Active"
                BookingId = Guid.NewGuid().ToString(), // Generate a unique BookingID
                Date = DateTime.Now,
                Money = money
            };

            if (bookingModel.FileImage != null && bookingModel.FileImage.Length > 0)
            {
                var filenameImageBookingModel = "Images/BookingImages/" + bookingModel.FileImage.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filenameImageBookingModel);
                using (var stream = System.IO.File.Create(path))
                {
                    await bookingModel.FileImage.CopyToAsync(stream);
                }
                booking.Images = filenameImageBookingModel;
            }
            else
            {
                return BadRequest("Please upload an image.");
            }

            _context.Bookings.Add(booking);

            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
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

        [HttpPut("ChangeBookingStatus/{bookingId}/{newStatus}")]
        public async Task<IActionResult> ChangeBookingStatus(string bookingId, string newStatus)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            // Kiểm tra trạng thái mới không được trống
            if (string.IsNullOrEmpty(newStatus))
            {
                return BadRequest("New status cannot be empty.");
            }

            // Cập nhật trạng thái
            booking.Status = newStatus;

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

        [HttpGet("GetWaitingBookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetWaitingBookings()
        {
            var waitingBookings = await _context.Bookings
                                                .Where(b => b.Status == "Waiting")
                                                .ToListAsync();

            if (waitingBookings == null || !waitingBookings.Any())
            {
                return NotFound("No bookings found with status Waiting.");
            }

            return waitingBookings;
        }
        [HttpGet("GetImage/{id}")]
        public async Task<IActionResult> GetImage(string id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null )
            {
                return NotFound("The object does not exist or has been deleted.");
            }
            string filename="";
            if (booking.Images == null || booking.Images.Length==0)
            {
                filename= "Images/common/noimage.png";
            }
            else
            {
                filename = booking.Images;
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

        [NonAction]
        // Method to check if a booking with the same CustomerId and ApartmentId already exists
        private bool BookingExistsForApartment(string customerId, string apartmentId)
        {
            return _context.Bookings.Any(b => b.CustomerId == customerId && b.ApartmentId == apartmentId);
        }

        private string GetFilePath(string filename) => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);
        // Method to check if a booking with the given BookingId already exists
        private bool BookingExists(string id)
        {
            return (_context.Bookings?.Any(e => e.BookingId == id)).GetValueOrDefault();
        }

    }
}
