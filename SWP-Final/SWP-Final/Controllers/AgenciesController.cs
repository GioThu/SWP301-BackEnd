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
    public class AgenciesController : ControllerBase
    {
        private readonly RealEasteSWPContext _context;

        public AgenciesController(RealEasteSWPContext context)
        {
            _context = context;
        }

        // GET: api/Agencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agency>>> GetAgencies()
        {
            if (_context.Agencies == null)
            {
                return NotFound();
            }

            var agencylist = await _context.Agencies.ToListAsync();

            // Check if the agency list is empty
            if (agencylist.Count == 0)
            {
                return NotFound("No agencies found.");
            }

            bool changesMade = false;
            foreach (var agency in agencylist)
            {
                if (agency.Images == null || agency.Images.Length == 0)
                {
                    agency.Images = "Images/common/noimage.png"; // Update with your default image path
                    changesMade = true;
                }
            }

            // Save changes if any agency was updated
            if (changesMade)
            {
                await _context.SaveChangesAsync();

            }

            return await _context.Agencies.ToListAsync();
        }

        // GET: api/Agencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Agency>> GetAgency(string id)
        {
            if (_context.Agencies == null)
            {
                return NotFound();
            }
            var agency = await _context.Agencies.FindAsync(id);

            if (agency == null)
            {
                return NotFound();
            }

            return agency;
        }


        // POST: api/Agencies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Agency>> PostAgency(Agency agency)
        {
            if (_context.Agencies == null)
            {
                return Problem("Entity set 'RealEasteSWPContext.Agencies'  is null.");
            }
            _context.Agencies.Add(agency);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AgencyExists(agency.AgencyId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAgency", new { id = agency.AgencyId }, agency);
        }

        // DELETE: api/Agencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgency(string id)
        {
            if (_context.Agencies == null)
            {
                return NotFound();
            }
            var agency = await _context.Agencies.FindAsync(id);
            if (agency == null)
            {
                return NotFound();
            }

            _context.Agencies.Remove(agency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //GET: api/Agencies/GetImage/
        [HttpGet("GetImage/{id}")]
        public async Task<IActionResult> GetImage(string id)
        {
            var agency = await _context.Agencies.FindAsync(id);
            if (agency == null || string.IsNullOrEmpty(agency.Images))
            {
                return NotFound("The image does not exist or has been deleted.");
            }

            var path = GetFilePath(agency.Images);
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



        [HttpPost("PostAgencyWithImage")]
        public async Task<IActionResult> PostInfoWithimageAsync([FromForm] AgencyRegisterModel agencyModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        UserId = Guid.NewGuid().ToString(),
                        Username = agencyModel.Username,
                        Password = agencyModel.Password,
                        RoleId = "Agency",
                         Status = "Active",
                        CreateDate = DateTime.Now
                    };

                    var agency = new Agency
                    {
                        AgencyId = Guid.NewGuid().ToString(),
                        FirstName = agencyModel.FirstName,
                        LastName = agencyModel.LastName,
                        Address = agencyModel.Address,
                        Phone = agencyModel.Phone,
                        UserId = user.UserId
                    };

                    if (agencyModel.FileImage != null && agencyModel.FileImage.Length > 0)
                    {
                        string filename = "Images/AgenciesImage/" + agencyModel.FileImage.FileName;

                        var path = GetFilePath(filename);
                        using (var stream = System.IO.File.Create(path))
                        {
                            await agencyModel.FileImage.CopyToAsync(stream);
                        }
                        agency.Images = filename;
                    }

                    _context.Users.Add(user);
                    _context.Agencies.Add(agency);
                    await _context.SaveChangesAsync();

                    // Remove circular references before serializing
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve
                    };

                    return Ok(JsonSerializer.Serialize(agency, options));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error while saving data: " + ex.Message);
                }
            }
            return BadRequest("Invalid model state.");
        }


        [HttpPost("PostAgencyWithNoImage")]
        public async Task<IActionResult> PostInfoWithNoImageAsync([FromForm] AgencyRegisterWithNoImageModel agencyModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        UserId = Guid.NewGuid().ToString(),
                        Username = agencyModel.Username,
                        Password = agencyModel.Password,
                        RoleId = "Agency",
                        Status = "Active",
                        CreateDate = DateTime.Now
                    };

                    var agency = new Agency
                    {
                        AgencyId = Guid.NewGuid().ToString(),
                        FirstName = agencyModel.FirstName,
                        LastName = agencyModel.LastName,
                        Address = agencyModel.Address,
                        Phone = agencyModel.Phone,
                        UserId = user.UserId
                    };
                    agency.Images = null;


                    _context.Users.Add(user);
                    _context.Agencies.Add(agency);
                    await _context.SaveChangesAsync();

                    // Remove circular references before serializing
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve
                    };

                    return Ok(JsonSerializer.Serialize(agency, options));
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error while saving data: " + ex.Message);
                }
            }
            return BadRequest("Invalid model state.");
        }




        //GET: api/Agencies/UploadImageNoImage
        [HttpGet("UploadImageNoImage")]
        public async Task<IActionResult> UploadImageNoImage()
        {
            var agencylist = await _context.Agencies.ToListAsync();

            // Check if the agency list is empty
            if (agencylist.Count == 0)
            {
                return NotFound("No agencies found.");
            }

            bool changesMade = false;
            foreach (var agency in agencylist)
            {
                if (agency.Images == null || agency.Images.Length == 0)
                {
                    agency.Images = "Images/common/noimage.png"; // Update with your default image path
                    changesMade = true;
                }
            }

            // Save changes if any agency was updated
            if (changesMade)
            {
                await _context.SaveChangesAsync();
                return Ok("Default images assigned to agencies without images.");
            }

            return Ok("No agencies needed updates.");
        }



        //POST: api/Agencies/UploadAgencyAndImage
        [HttpPost("UploadAgencyAndImage/{agencyid}")]
        public async Task<IActionResult> UploadAgencyAndImage([FromForm] AgencyModel agencyModel, string agencyid)
        {
            int count = 0;
            var agencylist = await _context.Agencies.ToListAsync();
            var agency = await _context.Agencies.FindAsync(agencyid);
            string filenameimageagency = agency.Images;
            if (agency == null)
            {
                return NotFound("Agency not found");
            }
            foreach (var agencyimage in agencylist)
            {
                if (agencyimage.Images == filenameimageagency)
                {
                    count++;
                }
            }

            // Check if an image file is provided
            if (agencyModel.FileImage != null && agencyModel.FileImage.Length > 0)
            {
                string filenameImageAgenciesModel = $"Images/AgenciesImage/{Path.GetFileName(agencyModel.FileImage.FileName)}";
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filenameImageAgenciesModel);

                // Ensure the directory exists
                var directoryName = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                // Save the new image
                using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    await agencyModel.FileImage.CopyToAsync(stream);
                }

                // Delete the old image if it is different from the new one and it's not the default image
                if (agency.Images != filenameImageAgenciesModel && agency.Images != valiablenoimage())
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", agency.Images);
                    if (System.IO.File.Exists(oldImagePath) && filenameimageagency != valiablenoimage() && count == 0)
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                agency.Images = filenameImageAgenciesModel;
            }

            // Update other agency details
            agency.FirstName = agencyModel.FirstName;
            agency.LastName = agencyModel.LastName;
            agency.Address = agencyModel.Address;
            agency.Phone = agencyModel.Phone;
            agency.Gender = agencyModel.Gender;

            await _context.SaveChangesAsync();
            return Ok(agency);
        }


        //DELETE: api/Agencies/DeleteImage
        [HttpDelete("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImage(string id)
        {
            int count = 0;
            var agencylist = await _context.Agencies.ToListAsync();
            var agency = await _context.Agencies.FindAsync(id);
            string filenameimageagency = agency.Images;
            if (agency == null || string.IsNullOrEmpty(agency.Images) || agency.Images == valiablenoimage())
            {
                return NotFound("Agency not found or image already removed.");
            }

            foreach (var agencyimage in agencylist)
            {
                if (agencyimage.Images == filenameimageagency)
                {
                    count++;
                }
            }

            if (count != 0)
            {
                agency.Images = valiablenoimage();
                await _context.SaveChangesAsync();
                return Ok("Image successfully deleted.");
            }

            var path = GetFilePath(agency.Images);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                // Optionally, update the agency object to reflect that the image has been removed
                agency.Images = null; // Assuming 'Images' is the property holding the image path. Adjust if necessary.
                _context.Agencies.Update(agency);
                await _context.SaveChangesAsync();
                return Ok("Image successfully deleted.");
            }
            else
            {
                return NotFound("File does not exist.");
            }
        }

        [HttpGet("GetAgencyStatistics")]
        public async Task<ActionResult<AgencyStatisticsModel>> GetAgencyStatistics(string agencyId)
        {
            var totalPosts = _context.Posts.Count(p => p.AgencyId == agencyId);
            var orderHistoryCount = _context.Orders.Count(o => o.AgencyId == agencyId);

            var agencyStatistics = new AgencyStatisticsModel
            {
                TotalPosts = totalPosts,
                OrderHistoryCount = orderHistoryCount
            };

            return agencyStatistics;
        }

        [HttpGet("GetListOrderWithAgencyID/{agencyId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetListOrderWithAgencyID(string agencyId)
        {
            // Retrieve orders associated with the specified agencyId
            var orders = await _context.Orders
                                       .Where(o => o.AgencyId == agencyId)
                                       .ToListAsync();

            if (orders == null)
            {
                return NotFound();
            }

            return orders;
        }

        [HttpGet("GetAgencyByUserID/{userId}")]
        public async Task<ActionResult<Agency>> GetAgencyByUserID(string userId)
        {
            // Retrieve the agency associated with the specified userId
            var agency = await _context.Agencies
                                        .FirstOrDefaultAsync(a => a.UserId == userId);

            if (agency == null)
            {
                return NotFound("No agency found for the specified user.");
            }

            return agency;
        }



        [HttpGet("GetAgencyNames")]
public async Task<ActionResult<IEnumerable<GetAgencyNameModel>>> GetAgencyNames()
{
    if (_context.Agencies == null)
    {
        return NotFound();
    }

    var agencyDetailsList = await _context.Agencies
        .Select(a => new GetAgencyNameModel
        {
            FirstName = a.FirstName,
            LastName = a.LastName,
            AgencyId = a.AgencyId
        })
        .ToListAsync();

    // Check if the agency details list is empty
    if (agencyDetailsList.Count == 0)
    {
        return NotFound("No agencies found.");
    }

    return agencyDetailsList;
}


        [NonAction]

        private string valiablenoimage() => "Images/common/noimage.png";

        private string GetFilePath(string filename) => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);


        private bool AgencyExists(string id)
        {
            return (_context.Agencies?.Any(e => e.AgencyId == id)).GetValueOrDefault();
        }
    }
}