using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ActivityModel = AcxiomCRM.Models.Activity;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class ActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string? search,
            string? status)
        {
            var query = _context.Activities
                .Include(x => x.Customer)
                .Include(x => x.Lead)
                .Include(x => x.Opportunity)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Subject.Contains(search) ||
                    x.ActivityType.Contains(search) ||
                    (x.Description != null &&
                     x.Description.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            var activities = await query
                .OrderByDescending(x => x.ActivityDate)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(activities);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var activity = await _context.Activities
                .Include(x => x.Customer)
                .Include(x => x.Lead)
                .Include(x => x.Opportunity)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (activity == null)
                return NotFound();

            return View(activity);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActivityModel activity)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(activity);
            }

            activity.CreatedDate = DateTime.UtcNow;

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var activity =
                await _context.Activities.FindAsync(id);

            if (activity == null)
                return NotFound();

            await LoadDropdowns();

            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ActivityModel activity)
        {
            if (id != activity.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(activity);
            }

            _context.Activities.Update(activity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var activity = await _context.Activities
                .Include(x => x.Customer)
                .Include(x => x.Lead)
                .Include(x => x.Opportunity)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (activity == null)
                return NotFound();

            return View(activity);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity =
                await _context.Activities.FindAsync(id);

            if (activity != null)
            {
                _context.Activities.Remove(activity);
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
