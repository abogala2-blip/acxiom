using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string? search,
            string? action)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a =>
                    a.EntityName.Contains(search) ||
                    (a.UserName != null &&
                     a.UserName.Contains(search)) ||
                    (a.Details != null &&
                     a.Details.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                query = query.Where(a => a.Action == action);
            }

            var logs = await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Action = action;

            return View(logs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var log = await _context.AuditLogs
                .FirstOrDefaultAsync(a => a.Id == id);

            if (log == null)
                return NotFound();

            return View(log);
        }
    }
}
