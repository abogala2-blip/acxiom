using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class LeadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------
        // INDEX
        // -------------------------------------------------
        public async Task<IActionResult> Index(
            string? search,
            string? status)
        {
            IQueryable<Lead> query = _context.Leads
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.LeadName.Contains(search) ||
                    x.Email.Contains(search) ||
                    (x.Phone != null && x.Phone.Contains(search)) ||
                    (x.CompanyName != null && x.CompanyName.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status) &&
                status != "All")
            {
                query = query.Where(x => x.Status == status);
            }

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(await query
                .OrderByDescending(x => x.Id)
                .ToListAsync());
        }

        // -------------------------------------------------
        // DETAILS
        // -------------------------------------------------
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var lead = await _context.Leads
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (lead == null)
                return NotFound();

            return View(lead);
        }

        // -------------------------------------------------
        // CREATE GET
        // -------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,SalesExecutive")]
        public IActionResult Create()
        {
            return View(new Lead());
        }

        // -------------------------------------------------
        // CREATE POST
        // -------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,SalesExecutive")]
        public async Task<IActionResult> Create(Lead lead)
        {
            if (!ModelState.IsValid)
            {
                return View(lead);
            }

            lead.CreatedDate = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(lead.Status))
                lead.Status = "New";

            if (string.IsNullOrWhiteSpace(lead.Source))
                lead.Source = "Website";

            if (User.IsInRole("SalesExecutive"))
            {
                lead.AssignedTo = User.Identity?.Name;
            }

            _context.Leads.Add(lead);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Lead added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------
        // EDIT GET
        // -------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,SalesExecutive")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var lead = await _context.Leads.FindAsync(id);

            if (lead == null)
                return NotFound();

            return View(lead);
        }

        // -------------------------------------------------
        // EDIT POST
        // -------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,SalesExecutive")]
        public async Task<IActionResult> Edit(
            int id,
            Lead lead)
        {
            if (id != lead.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(lead);

            var existing = await _context.Leads
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing == null)
                return NotFound();

            existing.LeadName = lead.LeadName;
            existing.Email = lead.Email;
            existing.Phone = lead.Phone;
            existing.CompanyName = lead.CompanyName;
            existing.Source = lead.Source;
            existing.Status = lead.Status;
            existing.ExpectedValue = lead.ExpectedValue;
            existing.Notes = lead.Notes;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Lead updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------
        // DELETE GET
        // -------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var lead = await _context.Leads
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (lead == null)
                return NotFound();

            return View(lead);
        }

        // -------------------------------------------------
        // DELETE POST
        // -------------------------------------------------
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lead = await _context.Leads.FindAsync(id);

            if (lead == null)
                return NotFound();

            _context.Leads.Remove(lead);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Lead deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
