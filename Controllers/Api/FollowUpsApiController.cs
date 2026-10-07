using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FollowUpsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FollowUpsApiController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var followUps =
                await _context.FollowUps
                    .Include(f => f.Customer)
                    .Include(f => f.Lead)
                    .Include(f => f.Opportunity)
                    .OrderBy(f => f.FollowUpDate)
                    .Select(f => new
                    {
                        f.Id,
                        f.Subject,
                        f.FollowUpDate,
                        f.Status,
                        f.Priority,
                        f.CustomerId,
                        f.LeadId,
                        f.OpportunityId,
                        f.Notes
                    })
                    .ToListAsync();

            return Ok(followUps);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var followUp =
                await _context.FollowUps
                    .FirstOrDefaultAsync(f => f.Id == id);

            if (followUp == null)
                return NotFound();

            return Ok(followUp);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            FollowUp followUp)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            followUp.Id = 0;

            followUp.CreatedDate =
                DateTime.UtcNow;

            _context.FollowUps.Add(followUp);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = followUp.Id },
                followUp);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            FollowUp input)
        {
            if (id != input.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var existing =
                await _context.FollowUps
                    .FirstOrDefaultAsync(f => f.Id == id);

            if (existing == null)
                return NotFound();

            existing.Subject = input.Subject;
            existing.FollowUpDate = input.FollowUpDate;
            existing.Status = input.Status;
            existing.Priority = input.Priority;
            existing.CustomerId = input.CustomerId;
            existing.LeadId = input.LeadId;
            existing.OpportunityId = input.OpportunityId;
            existing.Notes = input.Notes;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var followUp =
                await _context.FollowUps
                    .FirstOrDefaultAsync(f => f.Id == id);

            if (followUp == null)
                return NotFound();

            _context.FollowUps.Remove(followUp);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
