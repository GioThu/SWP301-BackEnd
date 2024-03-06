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
    public class UtilitiesController : ControllerBase
    {
        private readonly RealEasteSWPContext _context;

        public UtilitiesController(RealEasteSWPContext context)
        {
            _context = context;
        }

        // GET: api/Utilities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utility>>> GetUtilities()
        {
          if (_context.Utilities == null)
          {
              return NotFound();
          }
            return await _context.Utilities.ToListAsync();
        }

        // GET: api/Utilities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Utility>> GetUtility(string id)
        {
          if (_context.Utilities == null)
          {
              return NotFound();
          }
            var utility = await _context.Utilities.FindAsync(id);

            if (utility == null)
            {
                return NotFound();
            }

            return utility;
        }

        // PUT: api/Utilities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUtility(string id, Utility utility)
        {
            if (id != utility.UtilitiesId)
            {
                return BadRequest();
            }

            _context.Entry(utility).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UtilityExists(id))
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

        // POST: api/Utilities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Utility>> PostUtility(Utility utility)
        {
          if (_context.Utilities == null)
          {
              return Problem("Entity set 'RealEasteSWPContext.Utilities'  is null.");
          }
            _context.Utilities.Add(utility);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UtilityExists(utility.UtilitiesId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUtility", new { id = utility.UtilitiesId }, utility);
        }

        // DELETE: api/Utilities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUtility(string id)
        {
            if (_context.Utilities == null)
            {
                return NotFound();
            }
            var utility = await _context.Utilities.FindAsync(id);
            if (utility == null)
            {
                return NotFound();
            }

            _context.Utilities.Remove(utility);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UtilityExists(string id)
        {
            return (_context.Utilities?.Any(e => e.UtilitiesId == id)).GetValueOrDefault();
        }
    }
}
