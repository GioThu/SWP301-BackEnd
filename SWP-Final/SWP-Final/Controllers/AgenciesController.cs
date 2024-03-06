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

        // PUT: api/Agencies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgency(string id, Agency agency)
        {
            if (id != agency.AgencyId)
            {
                return BadRequest();
            }

            _context.Entry(agency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgencyExists(id))
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



        //POST: api/Agencies/PostImage
        [HttpPost("PostImage")]
        public async Task<IActionResult> PostInfoWithimageAsync([FromForm] AgencyModel agencyModel)
        {
            string filenameimageacenciesmodel = "Images/AgenciesImage/" + agencyModel.FileImage.FileName;
            if (ModelState.IsValid)
            {
                var agency = new Agency
                {
                    AgencyId = agencyModel.AgencyId,
                    FirstName = agencyModel.FirstName,

                };

                if (agencyModel.FileImage.Length > 0)
                {
                    var path = GetFilePath(filenameimageacenciesmodel);
                    using (var stream = System.IO.File.Create(path))
                    {
                        await agencyModel.FileImage.CopyToAsync(stream);
                    }
                    agency.Images = filenameimageacenciesmodel;
                }

                _context.Agencies.Add(agency);
                await _context.SaveChangesAsync();

                return Ok(agency);
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

        //POST: api/Agencies/UploadImage
        [HttpPost("UploadImage/{id}")]
        public async Task<IActionResult> UploadImageAsync([FromForm] AgencyModel agencyModel, string id)
        {
            int count = 0;
            var agencylist = await _context.Agencies.ToListAsync();
            string filenameimageacenciesmodel = $"Images/AgenciesImage/{agencyModel.FileImage.FileName}";
            var agency = await _context.Agencies.FindAsync(id);
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

            var path = GetFilePath(filenameimageagency);
            if (System.IO.File.Exists(path))
            {
                if (filenameimageagency != valiablenoimage() && count == 0)
                {
                    System.IO.File.Delete(path);
                }

            }
            var filepath = GetFilePath(filenameimageacenciesmodel);
            using (var stream = System.IO.File.Create(filepath))
            {
                await agencyModel.FileImage.CopyToAsync(stream);
            }
            agency.Images = filenameimageacenciesmodel;


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


        [NonAction]

        private string valiablenoimage() => "Images/common/noimage.png";

        private string GetFilePath(string filename) => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);


        private bool AgencyExists(string id)
        {
            return (_context.Agencies?.Any(e => e.AgencyId == id)).GetValueOrDefault();
        }
    }
}