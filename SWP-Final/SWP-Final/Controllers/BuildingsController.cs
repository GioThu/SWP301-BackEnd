using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SWP_Final.Entities;
using SWP_Final.Models;

namespace SWP_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingsController : ControllerBase
    {
        private readonly RealEasteSWPContext _context;

        public BuildingsController(RealEasteSWPContext context)
        {
            _context = context;
        }

        // GET: api/Buildings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Building>>> GetBuildings()
        {
            if (_context.Buildings == null)
            {
                return NotFound();
            }
            var buildingslist = await _context.Buildings.ToListAsync();

            // Check if the building list is empty
            if (buildingslist.Count == 0)
            {
                return NotFound("No agencies found.");
            }

            bool changesMade = false;
            foreach (var building in buildingslist)
            {
                if (building.Images == null || building.Images.Length == 0)
                {
                    building.Images = "Images/common/noimage.png"; // Update with your default image path
                    changesMade = true;
                }
            }

            // Save changes if any building was updated
            if (changesMade)
            {
                await _context.SaveChangesAsync();

            }
            return await _context.Buildings.ToListAsync();
        }

        // GET: api/Buildings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Building>> GetBuilding(string id)
        {
            if (_context.Buildings == null)
            {
                return NotFound();
            }
            var building = await _context.Buildings.FindAsync(id);

            if (building == null)
            {
                return NotFound();
            }

            return building;
        }

        // PUT: api/Buildings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBuilding(string id, Building building)
        {
            if (id != building.BuildingId)
            {
                return BadRequest();
            }

            _context.Entry(building).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuildingExists(id))
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

        //// POST: api/Buildings
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Building>> PostBuilding(Building building)
        //{
        //    if (_context.Buildings == null)
        //    {
        //        return Problem("Entity set 'RealEasteSWPContext.Buildings'  is null.");
        //    }
        //    _context.Buildings.Add(building);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (BuildingExists(building.BuildingId))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetBuilding", new { id = building.BuildingId }, building);
        //}

        // DELETE: api/Buildings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBuilding(string id)
        {
            if (_context.Buildings == null)
            {
                return NotFound();
            }
            var building = await _context.Buildings.FindAsync(id);
            if (building == null)
            {
                return NotFound();
            }

            _context.Buildings.Remove(building);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //GET: api/Buildings/GetImage/
        [HttpGet("GetImage/{id}")]
        public async Task<IActionResult> GetImageBuilding(string id)
        {
            var building = await _context.Buildings.FindAsync(id);
            if (building == null || string.IsNullOrEmpty(building.Images))
            {
                return NotFound("The image does not exist or has been deleted.");
            }

            var path = GetFilePath(building.Images);
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



        ////POST: api/Buildings/PostImage
        //[HttpPost("PostInfomationAndImage")]
        //public async Task<IActionResult> PostInfoWithimageBuilding([FromForm] BuildingModel buildingModel)
        //{
        //    string fileNameImageBuildingModel = "Images/BuildingImages/" + buildingModel.FileImage.FileName;
        //    if (ModelState.IsValid)
        //    {
        //        var building = new Building
        //        {
        //            BuildingId = buildingModel.BuildingId,
        //            Name = buildingModel.Name,
        //            Address = buildingModel.Address,
        //            TypeOfRealEstate = buildingModel.TypeOfRealEstate,
        //            NumberOfFloors = buildingModel.NumberOfFloors,
        //            NumberOfApartments = buildingModel.NumberOfApartments,
        //            Status = buildingModel.Status,
        //            YearOfConstruction = buildingModel.YearOfConstruction,
        //            Describe = buildingModel.Describe,
        //            Investor = buildingModel.Investor,
        //            Area = buildingModel.Area,
        //            Amenities = buildingModel.Amenities,

        //        };

        //        if (buildingModel.FileImage.Length > 0)
        //        {
        //            var path = GetFilePath(fileNameImageBuildingModel);
        //            using (var stream = System.IO.File.Create(path))
        //            {
        //                await buildingModel.FileImage.CopyToAsync(stream);
        //            }
        //            building.Images = fileNameImageBuildingModel;
        //        }

        //        _context.Buildings.Add(building);
        //        await _context.SaveChangesAsync();

        //        return Ok(building);
        //    }
        //    return BadRequest("Invalid model state.");
        //}

        //GET: api/Buildings/UploadImageNoImage
        //upload images no image when the image entry in the file is empty or null
        [HttpGet("UploadImageNoImage")]
        public async Task<IActionResult> UploadImageNoImage()
        {
            var buildinglist = await _context.Buildings.ToListAsync();

            // Check if the building list is empty
            if (buildinglist.Count == 0)
            {
                return NotFound("No agencies found.");
            }

            bool changesMade = false;
            foreach (var building in buildinglist)
            {
                if (building.Images == null || building.Images.Length == 0)
                {
                    building.Images = "Images/common/noimage.png"; // Update with your default image path
                    changesMade = true;
                }
            }

            // Save changes if any building was updated
            if (changesMade)
            {
                await _context.SaveChangesAsync();
                return Ok("Default images assigned to agencies without images.");
            }

            return Ok("No agencies needed updates.");
        }

        //POST: api/Buildings/UploadImage
        [HttpPost("UploadImage/{id}")]
        public async Task<IActionResult> UploadImageBuilding([FromForm] BuildingModel buildingModel, string id)
        {
            var building = await _context.Buildings.FindAsync(id);
            if (building == null)
            {
                return NotFound("Building not found");
            }

            // Only proceed with image processing if an image file is included in the request
            if (buildingModel.FileImage != null && buildingModel.FileImage.Length > 0)
            {
                string fileNameImageBuildingModel = $"Images/BuildingImages/{buildingModel.FileImage.FileName}";
                int count = _context.Buildings.Count(b => b.Images == building.Images);

                // Delete existing file if it's not used elsewhere
                var path = GetFilePath(building.Images);
                if (System.IO.File.Exists(path) && building.Images != valiablenoimage() && count == 0)
                {
                    System.IO.File.Delete(path);
                }

                // Save the new file
                var filepath = GetFilePath(fileNameImageBuildingModel);
                using (var stream = System.IO.File.Create(filepath))
                {
                    await buildingModel.FileImage.CopyToAsync(stream);
                }
                building.Images = fileNameImageBuildingModel;
            }

            await _context.SaveChangesAsync();
            return Ok(building);
        }


        //DELETE: api/Buildings/DeleteImage
        [HttpDelete("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImageBuilding(string id)
        {
            int count = 0;
            var buildinglist = await _context.Buildings.ToListAsync();
            var building = await _context.Buildings.FindAsync(id);
            string fileNameImageBuilding = building.Images;
            if (building == null || string.IsNullOrEmpty(building.Images) || building.Images == valiablenoimage())
            {
                return NotFound("building not found or image already removed.");
            }
            foreach (var buildingimage in buildinglist)
            {
                if (buildingimage.Images == fileNameImageBuilding)
                {
                    count++;
                    break;
                }
            }
            if (count != 0)
            {
                building.Images = valiablenoimage();
                await _context.SaveChangesAsync();
                return Ok("Image successfully deleted.");
            }
            var path = GetFilePath(building.Images);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                // Optionally, update the building object to reflect that the image has been removed
                building.Images = null; // Assuming 'Images' is the property holding the image path. Adjust if necessary.
                _context.Buildings.Update(building);
                await _context.SaveChangesAsync();
                return Ok("Image successfully deleted.");
            }
            else
            {
                return NotFound("File does not exist.");
            }
        }

        [HttpPost("PostInfomationAndImage")]
        public async Task<IActionResult> PostInfoWithimageBuilding([FromForm] AddBuildingModel buildingModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model state.");
            }

            // Kiểm tra tính hợp lệ của số lầu và số căn hộ trên mỗi tầng
            if (buildingModel.NumberOfFloor <= 0 || buildingModel.NumberOfApartment <= 0)
            {
                return BadRequest("Number of floors and number of apartments per floor must be greater than zero.");
            }

            string fileNameImageBuildingModel = "Images/BuildingImages/" + buildingModel.FileImage.FileName;


            var building = new Building
            {
                ProjectId = buildingModel.ProjectId,
                BuildingId = Guid.NewGuid().ToString(),
                Name = buildingModel.Name,
                NumberOfFloors = buildingModel.NumberOfFloor,
                NumberOfApartments = buildingModel.NumberOfApartment,
                Describe = buildingModel.Description,
                Images = fileNameImageBuildingModel
            };

            // Lưu hình ảnh của tòa nhà
            if (buildingModel.FileImage.Length > 0)
            {
                var path = GetFilePath(fileNameImageBuildingModel);
                using (var stream = System.IO.File.Create(path))
                {
                    await buildingModel.FileImage.CopyToAsync(stream);
                }
            }

            _context.Buildings.Add(building);
            await _context.SaveChangesAsync();

            // Tạo các căn hộ tương ứng với số lầu và số lượng căn hộ mỗi lầu
            for (int i = 1; i <= building.NumberOfFloors; i++)
            {
                for (int j = 1; j <= building.NumberOfApartments / building.NumberOfFloors; j++)
                {
                    string apartmentId;
                    if (j < 10)
                    {
                        apartmentId = $"{building.BuildingId}?{i}0{j}";
                    }
                    else
                    {
                        apartmentId = $"{building.BuildingId}?{i}{j}";
                    }
                    var apartment = new Apartment
                    {
                        ApartmentId = apartmentId,
                        BuildingId = building.BuildingId,
                        ApartmentType = "Images/common/noimage.png",
                        Area = null,
                        FloorNumber = i,
                        AgencyId = null,
                        Status = null
                    };
                    _context.Apartments.Add(apartment);
                }
            }

            await _context.SaveChangesAsync();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            // Serialize the Building object to JSON with the specified options
            string jsonString = JsonSerializer.Serialize(building, options);

            // Return the JSON string
            return Ok();
        }

        [HttpGet("GetListBuildingDetails")]
        public async Task<ActionResult<IEnumerable<ListBuildingDetailsModel>>> GetListBuildingDetails()
        {
            var buildingDetails = await _context.Buildings
                                                .Include(b => b.Project) // Include the Project entity
                                                .Select(b => new ListBuildingDetailsModel
                                                {
                                                    BuildingId = b.BuildingId,
                                                    ProjectName = b.Project.Name,
                                                    BuildingName = b.Name,
                                                    NumberOfFloors = b.NumberOfFloors ?? 0,
                                                    NumberOfApartments = b.NumberOfApartments ?? 0
                                                }).ToListAsync();

            if (buildingDetails == null)
            {
                return NotFound();
            }

            return buildingDetails;
        }

        [HttpGet("GetProjectBuildingDetails")]
        public async Task<ActionResult<IEnumerable<ProjectBuildingDetailsModel>>> GetProjectBuildingDetails()
        {
            var projectBuildingDetailsList = await _context.Buildings
                .Select(b => new ProjectBuildingDetailsModel
                {
                    ProjectId = b.ProjectId,
                    ProjectName = b.Project.Name, // Accessing ProjectName from the associated Project entity
                    BuildingId = b.BuildingId,
                    BuildingName = b.Name
                }).ToListAsync();

            if (projectBuildingDetailsList == null)
            {
                return NotFound();
            }

            return projectBuildingDetailsList;
        }


        [HttpPost("DistributeFloor")]
        public async Task<IActionResult> DistributeFloor(string buildingId, string agencyId, int floor, decimal price)
        {
            var apartmentsInBuilding = await _context.Apartments
        .Where(a => a.BuildingId == buildingId)
        .ToListAsync();

            if (apartmentsInBuilding == null || apartmentsInBuilding.Count == 0)
            {
                return NotFound($"Không tìm thấy căn hộ nào cho tòa nhà có ID: {buildingId}");
            }

            // Cập nhật AgencyId và Status cho từng căn hộ trong tòa nhà
            foreach (var apartment in apartmentsInBuilding)
            {
                if (apartment.FloorNumber == floor) 
                {
                    apartment.AgencyId = agencyId;
                    apartment.Status = "Distributed";
                    apartment.Price = price;
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return Ok();
        }






        [NonAction]
        private string valiablenoimage() => "Images/common/noimage.png";
        private string GetFilePath(string filename) => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);

        


        private bool BuildingExists(string id)
        {
            return (_context.Buildings?.Any(e => e.BuildingId == id)).GetValueOrDefault();
        }
    }
}