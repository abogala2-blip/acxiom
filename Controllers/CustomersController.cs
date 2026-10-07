using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin =>
            User.IsInRole("Admin");

        private bool IsManager =>
            User.IsInRole("Manager");

        private bool IsSalesExecutive =>
            User.IsInRole("SalesExecutive");

        private IQueryable<Customer> ScopedCustomers()
        {
            var query =
                _context.Customers.AsNoTracking();

            if (IsAdmin || IsManager)
            {
                return query;
            }

            if (IsSalesExecutive)
            {
                var email =
                    User.Identity?.Name ?? "";

                return query.Where(
                    x => x.AssignedTo == email);
            }

            return query.Where(x => false);
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            string? status)
        {
            var query = ScopedCustomers();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(c =>
                    c.FullName.Contains(search) ||
                    c.Email.Contains(search) ||
                    (c.Phone != null &&
                     c.Phone.Contains(search)) ||
                    (c.Company != null &&
                     c.Company.Contains(search)) ||
                    (c.City != null &&
                     c.City.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status) &&
                status != "All")
            {
                query = query.Where(
                    c => c.Status == status);
            }

            ViewBag.Search = search;
            ViewBag.Status = status;

            var customers =
                await query
                    .OrderByDescending(c => c.Id)
                    .ToListAsync();

            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var customer =
                await ScopedCustomers()
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [Authorize(Roles = "Admin,Manager,SalesExecutive")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Customer());
        }

        [Authorize(Roles = "Admin,Manager,SalesExecutive")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            if (IsSalesExecutive)
            {
                customer.AssignedTo =
                    User.Identity?.Name;
            }

            customer.CreatedAt =
                DateTime.UtcNow;

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Manager,SalesExecutive")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var customer =
                await ScopedCustomers()
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [Authorize(Roles = "Admin,Manager,SalesExecutive")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "Id,FullName,Email,Phone,Company,City,Status,AssignedTo,CreatedAt")]
            Customer customer)
        {
            if (id != customer.Id)
                return NotFound();

            var existing =
                await ScopedCustomers()
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (existing == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(customer);

            existing.FullName = customer.FullName;
            existing.Email = customer.Email;
            existing.Phone = customer.Phone;
            existing.Company = customer.Company;
            existing.City = customer.City;
            existing.Status = customer.Status;

            if (IsAdmin || IsManager)
            {
                existing.AssignedTo =
                    customer.AssignedTo;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var customer =
                await _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var customer =
                await _context.Customers
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
