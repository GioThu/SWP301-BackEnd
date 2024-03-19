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
    public class OrdersController : ControllerBase
    {
        private readonly RealEasteSWPContext _context;

        public OrdersController(RealEasteSWPContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
          if (_context.Orders == null)
          {
              return NotFound();
          }
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(string id)
        {
          if (_context.Orders == null)
          {
              return NotFound();
          }
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(string id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
          if (_context.Orders == null)
          {
              return Problem("Entity set 'RealEasteSWPContext.Orders'  is null.");
          }
            _context.Orders.Add(order);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderExists(order.OrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(string id)
        {
            return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }



        [HttpPost("CreateOrderFromBooking/{bookingId}")]
        public async Task<IActionResult> CreateOrderFromBooking(string bookingId)
        {
            // Find the booking
            var booking = await _context.Bookings.FindAsync(bookingId);

            if (booking == null)
            {
                return NotFound($"Booking with ID {bookingId} not found.");
            }

            // Retrieve all bookings with the same apartmentId as the one in the provided booking
            var bookingsToClose = await _context.Bookings
                .Where(b => b.ApartmentId == booking.ApartmentId && b.Status == "Active")
                .ToListAsync();

            // Close each booking found except the current one
            foreach (var b in bookingsToClose)
            {
                if (b.BookingId != bookingId)
                {
                    b.Status = "Closed";
                }
                else
                {
                    b.Status = "Complete"; // Change the status of the current booking to "Complete"
                }
            }

            var apartment = await _context.Apartments.FindAsync(booking.ApartmentId);

            if (apartment == null)
            {
                return NotFound($"Apartment with ID {booking.ApartmentId} not found.");
            }

            // Change the status of the apartment to "Sold"
            apartment.Status = "Sold";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Failed to update bookings status.");
            }

            // Create an order from the provided booking
            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString(), 
                Date = booking.Date,
                AgencyId = booking.AgencyId,
                ApartmentId = booking.ApartmentId,
                Status = "Waiting", 
                TotalAmount = booking.Money
            };

            // Add the order to the context
            _context.Orders.Add(order);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Failed to create order.");
            }

            return Ok("Order created successfully.");
        }

        [HttpDelete("DeleteOrderAndHealingBooking/{bookingId}")]
        public async Task<IActionResult> DeleteOrderAndHealingBooking(string bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);

            if (booking == null)
            {
                return NotFound($"Booking with ID {bookingId} not found.");
            }

            var apartment = await _context.Apartments.FindAsync(booking.ApartmentId);

            if (apartment == null)
            {
                return NotFound($"Apartment with ID {booking.ApartmentId} not found.");
            }

            var orderToDelete = await _context.Orders
               .Where(o => o.ApartmentId == booking.ApartmentId)
               .FirstOrDefaultAsync();

            var bookingsToDelete = await _context.Bookings
               .Where(b => b.ApartmentId == booking.ApartmentId)
               .ToListAsync();

            apartment.Status = "Distributed"; // Chuyển trạng thái của apartment thành "Updated"

            _context.Orders.Remove(orderToDelete);
            await _context.SaveChangesAsync();

            _context.Bookings.RemoveRange(bookingsToDelete);
            await _context.SaveChangesAsync();

            return Ok("Orders deleted successfully.");
        }

        [HttpGet("GetAllOderByAgencyId/{agencyId}")]
        public async Task<ActionResult<IEnumerable<OrdersHistoryModel>>> GetAllOderByAgencyId(string agencyId)
        {
            // Retrieve orders with the specified agencyId
            var orders = await _context.Orders
                                        .Where(b => b.AgencyId == agencyId)
                                        .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the specified agency.");
            }

            var ordersHistory = new List<OrdersHistoryModel>();

            foreach (var order in orders)
            {

                var orderHistory = new OrdersHistoryModel
                {
                    OrderId = order.OrderId,
                    Date = order.Date,
                    AgencyId = order.AgencyId,
                    ApartmentId = order.ApartmentId,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    CustomerId = order.CustomerId
                };

                ordersHistory.Add(orderHistory);
            }

            return ordersHistory;
        }

        [HttpGet("GetAllOderByCustomerId/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrdersHistoryModel>>> GetAllOderByCustomerId(string customerId)
        {
            var orders = await _context.Orders
                               .Where(o => o.CustomerId == customerId )
                               .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the specified customer.");
            }

            var ordersHistory = orders.Select(order => new OrdersHistoryModel
            {
                OrderId = order.OrderId,
                Date = order.Date,
                AgencyId = order.AgencyId,
                ApartmentId = order.ApartmentId,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CustomerId = order.CustomerId
            }).ToList();

            return ordersHistory;
        }

        [HttpGet("GetAllOderByOrderId/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrdersHistoryModel>>> GetAllOderByOrderId(string orderId)
        {
            // Retrieve bookings with the specified customerId and status "Complete"
            var orders = await _context.Orders
                                        .Where(b => b.OrderId == orderId)
                                        .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the specified agency.");
            }

            var ordersBill = new List<OrdersHistoryModel>();

            foreach (var order in orders)
            {
                // Find complete booking for the current order's apartment
                var completeBooking = await _context.Bookings
                                            .FirstOrDefaultAsync(b => b.ApartmentId == order.ApartmentId && b.Status == "Complete");

                if (completeBooking == null)
                {
                    return NotFound("No complete booking found for the specified agency.");
                }

                // Map Order entity to OrdersHistoryModel instance
                var orderBill = new OrdersHistoryModel
                {
                    OrderId = order.OrderId,
                    Date = order.Date,
                    AgencyId = order.AgencyId,
                    ApartmentId = order.ApartmentId,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    CustomerId = completeBooking.CustomerId
                };

                ordersBill.Add(orderBill);
            }

            return ordersBill;
        }

        [HttpPut("ChangeOrderStatus/{orderId}/{newStatus}")]
        public async Task<IActionResult> ChangeOrderStatus(string orderId, string newStatus)
        {
            // Find the order
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            // Update the order status
            order.Status = newStatus;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Failed to update order status.");
            }

            return Ok($"Order status changed to {newStatus}.");
        }

        [HttpGet("GetWaitingOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetWaitingOrders()
        {
            var orders = await _context.Orders
                                        .Where(o => o.Status == "Waiting")
                                        .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found with status Waiting.");
            }

            return orders;
        }


    }
}


