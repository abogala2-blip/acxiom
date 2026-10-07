using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var users = await _userManager.Users
                .OrderBy(x => x.Email)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                users = users
                    .Where(x =>
                        (x.Email ?? "")
                            .Contains(
                                search,
                                StringComparison.OrdinalIgnoreCase)
                        ||
                        (x.FullName ?? "")
                            .Contains(
                                search,
                                StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.Search = search;

            var rows = new List<UserRow>();

            foreach (var user in users)
            {
                var roles =
                    await _userManager.GetRolesAsync(user);

                rows.Add(new UserRow
                {
                    Id = user.Id,
                    FullName = user.FullName ?? "",
                    Email = user.Email ?? "",
                    Roles = string.Join(", ", roles)
                });
            }

            return View(rows);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles =
                await _roleManager.Roles
                    .OrderBy(x => x.Name)
                    .Select(x => x.Name!)
                    .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string fullName,
            string email,
            string password,
            string role)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                TempData["UserError"] =
                    "All fields are required.";

                return RedirectToAction(nameof(Create));
            }

            var existing =
                await _userManager.FindByEmailAsync(email);

            if (existing != null)
            {
                TempData["UserError"] =
                    "An account with this email already exists.";

                return RedirectToAction(nameof(Create));
            }

            var user = new ApplicationUser
            {
                UserName = email.Trim(),
                Email = email.Trim(),
                FullName = fullName.Trim(),
                EmailConfirmed = true
            };

            var result =
                await _userManager.CreateAsync(
                    user,
                    password);

            if (!result.Succeeded)
            {
                TempData["UserError"] =
                    string.Join(
                        " ",
                        result.Errors.Select(x => x.Description));

                return RedirectToAction(nameof(Create));
            }

            if (await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(
                    user,
                    role);
            }

            TempData["UserSuccess"] =
                $"Account created successfully for {email}.";

            return RedirectToAction(nameof(Index));
        }
    }

    public class UserRow
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
    }
}
