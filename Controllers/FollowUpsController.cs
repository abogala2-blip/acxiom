using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class FollowUpsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FollowUpsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, string? status)
        {
            var query = _context.FollowUps
                .Include(x => x.Customer)
                .Include(x => x.Lead)
                .Include(x => x.Opportunity)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Subject.Contains(search) ||
                    (x.Notes != null && x.Notes.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            var followUps = await query
                .OrderBy(x => x.FollowUpDate)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(followUps);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var followUp = await _context.FollowUps
                .Include(x => x.Customer)
                .Include(x => x.Lead)
                .Include(x => x.Opportunity)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (followUp == null)
                return NotFound();

            return View(followUp);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FollowUp followUp)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(followUp);
            }

            followUp.CreatedDate = DateTime.UtcNow;

            _context.FollowUps.Add(followUp);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var followUp = await _context.FollowUps.FindAsync(id);

            if (followUp == null)
                return NotFound();

            await LoadDropdowns();

            return View(followUp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FollowUp followUp)
        {
            if (id != followUp.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(followUp);
            }

            _context.FollowUps.Update(followUp);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var followUp = await _context.FollowUps
                .Include(x => x.Customer)
                .Include(x => x.Lead)
                .Include(x => x.Opportunity)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (followUp == null)
                return NotFound();

            return View(followUp);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var followUp = await _context.FollowUps.FindAsync(id);

            if (followUp != null)
            {
                _context.FollowUps.Remove(followUp);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            ViewBag.Customers = await _context.Customers
                .OrderBy(x => x.FullName)
                .ToListAsync();

            ViewBag.Leads = await _context.Leads
                .OrderBy(x => x.LeadName)
                .ToListAsync();

            ViewBag.Opportunities = await _context.Opportunities
                .OrderBy(x => x.OpportunityName)
                .ToListAsync();
        }
    }
}