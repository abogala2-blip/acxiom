using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AcxiomCRM.Models;

namespace AcxiomCRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();

            var rows = new List<RoleRow>();

            foreach (var role in roles)
            {
                var users =
                    await _userManager.GetUsersInRoleAsync(
                        role.Name!);

                rows.Add(new RoleRow
                {
                    Role = role.Name!,
                    Users = users.Count
                });
            }

            return View(rows);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                TempData["RoleError"] =
                    "Please enter a role name.";

                return RedirectToAction(nameof(Index));
            }

            roleName = roleName.Trim();

            if (await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["RoleError"] =
                    "This role already exists.";

                return RedirectToAction(nameof(Index));
            }

            var result =
                await _roleManager.CreateAsync(
                    new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                TempData["RoleError"] =
                    string.Join(
                        " ",
                        result.Errors.Select(x => x.Description));

                return RedirectToAction(nameof(Index));
            }

            // Automatically create a demo account for the new role.
            var clean =
                new string(
                    roleName
                        .ToLowerInvariant()
                        .Where(char.IsLetterOrDigit)
                        .ToArray());

            if (string.IsNullOrWhiteSpace(clean))
            {
                clean = "role";
            }

            var email = $"{clean}@acxiomcrm.com";
            var password = "Role@123";

            var existing =
                await _userManager.FindByEmailAsync(email);

            if (existing == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = $"{roleName} Demo User",
                    EmailConfirmed = true
                };

                var createResult =
                    await _userManager.CreateAsync(
                        user,
                        password);

                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(
                        user,
                        roleName);
                }
            }

            TempData["RoleSuccess"] =
                $"Role '{roleName}' created. Demo login: {email} / {password}";

            return RedirectToAction(nameof(Index));
        }
    }

    public class RoleRow
    {
        public string Role { get; set; } = string.Empty;
        public int Users { get; set; }
    }
}
