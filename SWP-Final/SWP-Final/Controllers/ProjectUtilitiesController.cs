using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_Final.Entities;

namespace SWP_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectUtilitiesController : ControllerBase
    {
        private readonly RealEasteSWPContext _context;

        public ProjectUtilitiesController(RealEasteSWPContext context)
        {
            _context = context;
        }

        // GET: api/ProjectUtilities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectUtility>>> GetProjectUtilities()
        {
          if (_context.ProjectUtilities == null)
          {
              return NotFound();
          }
            return await _context.ProjectUtilities.ToListAsync();
        }

        // GET: api/ProjectUtilities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectUtility>> GetProjectUtility(string id)
        {
          if (_context.ProjectUtilities == null)
          {
              return NotFound();
          }
            var projectUtility = await _context.ProjectUtilities.FindAsync(id);

            if (projectUtility == null)
            {
                return NotFound();
            }

            return projectUtility;
        }

        // PUT: api/ProjectUtilities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectUtility(string id, ProjectUtility projectUtility)
        {
            if (id != projectUtility.ProjectUtilitiesId)
            {
                return BadRequest();
            }

            _context.Entry(projectUtility).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectUtilityExists(id))
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

        // POST: api/ProjectUtilities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectUtility>> PostProjectUtility(ProjectUtility projectUtility)
        {
          if (_context.ProjectUtilities == null)
          {
              return Problem("Entity set 'RealEasteSWPContext.ProjectUtilities'  is null.");
          }
            _context.ProjectUtilities.Add(projectUtility);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProjectUtilityExists(projectUtility.ProjectUtilitiesId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetProjectUtility", new { id = projectUtility.ProjectUtilitiesId }, projectUtility);
        }

        // DELETE: api/ProjectUtilities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectUtility(string id)
        {
            if (_context.ProjectUtilities == null)
            {
                return NotFound();
            }
            var projectUtility = await _context.ProjectUtilities.FindAsync(id);
            if (projectUtility == null)
            {
                return NotFound();
            }

            _context.ProjectUtilities.Remove(projectUtility);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectUtilityExists(string id)
        {
            return (_context.ProjectUtilities?.Any(e => e.ProjectUtilitiesId == id)).GetValueOrDefault();
        }
    }
}
