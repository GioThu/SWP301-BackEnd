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
    public class CustomersController : ControllerBase
    {
        private readonly RealEasteSWPContext _context;

        public CustomersController(RealEasteSWPContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }

            var customerslist = await _context.Customers.ToListAsync();

            // Check if the customer list is empty
            if (customerslist.Count == 0)
            {
                return NotFound("No agencies found.");
            }

            bool changesMade = false;
            foreach (var customer in customerslist)
            {
                if (customer.Images == null || customer.Images.Length == 0)
                {
                    customer.Images = "Images/common/noimage.png"; // Update with your default image path
                    changesMade = true;
                }
            }

            // Save changes if any customer was updated
            if (changesMade)
            {
                await _context.SaveChangesAsync();

            }
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(string id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(string id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'RealEasteSWPContext.Customers'  is null.");
            }
            _context.Customers.Add(customer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(customer.CustomerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //GET: api/Customers/GetImage/
        [HttpGet("GetImage/{id}")]
        public async Task<IActionResult> GetImageCustomer(string id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null || string.IsNullOrEmpty(customer.Images))
            {
                return NotFound("The image does not exist or has been deleted.");
            }

            var path = GetFilePath(customer.Images);
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



        //POST: api/Customers/PostImage
        [HttpPost("PostImage")]
        public async Task<IActionResult> PostInfoWithimageCustomer([FromForm] CustomerModel customerModel)
        {
            string filenameimageacenciesmodel = "Images/CustomerImages/" + customerModel.FileImage.FileName;
            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    CustomerId = customerModel.CustomerId,
                    FirstName = customerModel.FirstName,

                };

                if (customerModel.FileImage.Length > 0)
                {
                    var path = GetFilePath(filenameimageacenciesmodel);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await customerModel.FileImage.CopyToAsync(stream);
                    }
                    customer.Images = filenameimageacenciesmodel;
                }

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return Ok(customer);
            }
            return BadRequest("Invalid model state.");
        }

        //GET: api/Customers/UploadImageNoImage
        //upload images no image when the image entry in the file is empty or null
        [HttpGet("UploadImageNoImage")]
        public async Task<IActionResult> UploadImageNoImage()
        {
            var customerlist = await _context.Customers.ToListAsync();

            // Check if the customer list is empty
            if (customerlist.Count == 0)
            {
                return NotFound("No agencies found.");
            }

            bool changesMade = false;
            foreach (var customer in customerlist)
            {
                if (customer.Images == null || customer.Images.Length == 0)
                {
                    customer.Images = "Images/common/noimage.png"; // Update with your default image path
                    changesMade = true;
                }
            }

            // Save changes if any customer was updated
            if (changesMade)
            {
                await _context.SaveChangesAsync();
                return Ok("Default images assigned to agencies without images.");
            }

            return Ok("No agencies needed updates.");
        }

        //POST: api/Customers/UploadInformationAndImage
        [HttpPost("UploadInformationAndImage/{customerid}")]
        public async Task<IActionResult> UploadInformationAndImage([FromForm] CustomerModel customerModel, string customerid)
        {
            var customer = await _context.Customers.FindAsync(customerid);
            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            // Initialize count to 0
            int count = 0;

            // Retrieve all customers
            var customerList = await _context.Customers.ToListAsync();

            // Retrieve the existing image filename for comparison
            string filenameImageCustomerModel = customer.Images;

            // Loop through existing customers to count occurrences of the same image filename
            foreach (var customerImage in customerList)
            {
                if (customerImage.Images == filenameImageCustomerModel)
                {
                    count++;
                }
            }

            // Only proceed with image processing if an image file is included and has content
            if (customerModel.FileImage != null && customerModel.FileImage.Length > 0)
            {
                // Construct the new image filename
                string filenameImageCustomersModel = $"Images/CustomerImages/{Path.GetFileName(customerModel.FileImage.FileName)}";

                // Get the file path for saving
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filenameImageCustomersModel);

                // Ensure the directory exists
                var directoryName = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                // Save the new file
                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    await customerModel.FileImage.CopyToAsync(stream);
                }

                // Delete the old file if it's not the default image and not used elsewhere
                if (customer.Images != filenameImageCustomersModel && customer.Images != valiablenoimage() && count == 0)
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", customer.Images);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Update the customer's image path
                customer.Images = filenameImageCustomersModel;
            }

            // Always update these fields regardless of whether a new image was uploaded
            customer.FirstName = customerModel.FirstName;
            customer.LastName = customerModel.LastName;
            customer.Address = customerModel.Address;
            customer.Gender = customerModel.Gender;
            customer.Phone = customerModel.Phone;

            await _context.SaveChangesAsync();
            return Ok(customer);
        }



        //DELETE: api/Customers/DeleteImage
        [HttpDelete("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImageCustomer(string id)
        {
            int count = 0;
            var customerlist = await _context.Customers.ToListAsync();
            var customer = await _context.Customers.FindAsync(id);
            string filenameimagecustomer = customer.Images;
            if (customer == null || string.IsNullOrEmpty(customer.Images) || customer.Images == valiablenoimage())
            {
                return NotFound("customer not found or image already removed.");
            }
            foreach (var customerimage in customerlist)
            {
                if (customerimage.Images == filenameimagecustomer)
                {
                    count++;
                    break;
                }
            }
            if (count != 0)
            {
                customer.Images = valiablenoimage();
                await _context.SaveChangesAsync();
                return Ok("Image successfully deleted.");
            }
            var path = GetFilePath(customer.Images);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                // Optionally, update the customer object to reflect that the image has been removed
                customer.Images = null; // Assuming 'Images' is the property holding the image path. Adjust if necessary.
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
                return Ok("Image successfully deleted.");
            }
            else
            {
                return NotFound("File does not exist.");
            }
        }

        [HttpGet("GetCustomerByUserID/{userId}")]
        public async Task<ActionResult<Customer>> GetCustomerByUserID(string userId)
        {
            // Retrieve the customer associated with the specified userId
            var customer = await _context.Customers
                                        .FirstOrDefaultAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return NotFound("No customer found for the specified user.");
            }

            return customer;
        }



        [NonAction]
        private string valiablenoimage() => "Images/common/noimage.png";
        private string GetFilePath(string filename) => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);


        private bool CustomerExists(string id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}