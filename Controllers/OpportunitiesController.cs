using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class OpportunitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OpportunitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // INDEX
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            string? stage,
            string? status)
        {
            var query = _context.Opportunities
                .Include(o => o.Customer)
                .Include(o => o.Lead)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o =>
                    o.OpportunityName.Contains(search) ||
                    (o.Customer != null &&
                     o.Customer.FullName.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(stage))
            {
                query = query.Where(o => o.Stage == stage);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status);
            }

            var opportunities = await query
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Stage = stage;
            ViewBag.Status = status;

            return View(opportunities);
        }

        // =====================================================
        // DETAILS
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var opportunity = await _context.Opportunities
                .Include(o => o.Customer)
                .Include(o => o.Lead)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return NotFound();

            return View(opportunity);
        }

        // =====================================================
        // CREATE - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCustomers();

            return View(new Opportunity
            {
                Stage = "Qualification",
                Probability = 25,
                Status = "Open",
                ExpectedCloseDate = DateTime.Today.AddDays(30)
            });
        }

        // =====================================================
        // CREATE - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Opportunity opportunity)
        {
            if (opportunity.Amount <= 0)
            {
                ModelState.AddModelError(
                    nameof(opportunity.Amount),
                    "Amount must be greater than zero.");
            }

            if (opportunity.Probability < 0 ||
                opportunity.Probability > 100)
            {
                ModelState.AddModelError(
                    nameof(opportunity.Probability),
                    "Probability must be between 0 and 100.");
            }

            if (opportunity.Status == "Open" &&
                opportunity.ExpectedCloseDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(opportunity.ExpectedCloseDate),
                    "Open opportunities cannot have a past close date.");
            }

            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == opportunity.CustomerId);

            if (!customerExists)
            {
                ModelState.AddModelError(
                    nameof(opportunity.CustomerId),
                    "Please select a valid customer.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCustomers();
                return View(opportunity);
            }

            opportunity.CreatedDate = DateTime.UtcNow;

            _context.Opportunities.Add(opportunity);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // EDIT - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return NotFound();

            await LoadCustomers();

            return View(opportunity);
        }

        // =====================================================
        // EDIT - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Opportunity opportunity)
        {
            if (id != opportunity.Id)
                return NotFound();

            if (opportunity.Amount <= 0)
            {
                ModelState.AddModelError(
                    nameof(opportunity.Amount),
                    "Amount must be greater than zero.");
            }

            if (opportunity.Probability < 0 ||
                opportunity.Probability > 100)
            {
                ModelState.AddModelError(
                    nameof(opportunity.Probability),
                    "Probability must be between 0 and 100.");
            }

            if (opportunity.Status == "Open" &&
                opportunity.ExpectedCloseDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(opportunity.ExpectedCloseDate),
                    "Open opportunities cannot have a past close date.");
            }

            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == opportunity.CustomerId);

            if (!customerExists)
            {
                ModelState.AddModelError(
                    nameof(opportunity.CustomerId),
                    "Please select a valid customer.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCustomers();
                return View(opportunity);
            }

            var existing = await _context.Opportunities
                .FirstOrDefaultAsync(o => o.Id == id);

            if (existing == null)
                return NotFound();

            existing.OpportunityName = opportunity.OpportunityName;
            existing.CustomerId = opportunity.CustomerId;
            existing.LeadId = opportunity.LeadId;
            existing.Amount = opportunity.Amount;
            existing.Stage = opportunity.Stage;
            existing.Probability = opportunity.Probability;
            existing.ExpectedCloseDate = opportunity.ExpectedCloseDate;
            existing.Status = opportunity.Status;
            existing.AssignedTo = opportunity.AssignedTo;
            existing.Notes = opportunity.Notes;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // DELETE - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var opportunity = await _context.Opportunities
                .Include(o => o.Customer)
                .Include(o => o.Lead)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return NotFound();

            return View(opportunity);
        }

        // =====================================================
        // DELETE - POST
        // =====================================================

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return NotFound();

            _context.Opportunities.Remove(opportunity);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // LOAD CUSTOMERS
        // =====================================================

        private async Task LoadCustomers()
        {
            ViewBag.Customers = await _context.Customers
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }
    }
}
