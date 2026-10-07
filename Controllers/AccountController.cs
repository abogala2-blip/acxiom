using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AcxiomCRM.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        // ----------------------------------------------------
        // LOGIN PAGE
        // ----------------------------------------------------

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        // ----------------------------------------------------
        // LOGIN POST
        // ----------------------------------------------------

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email.Trim());

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName ?? model.Email.Trim(),
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                _context.AuditLogs.Add(new AuditLog
                {
                    UserName = user.UserName,
                    Action = "Login",
                    EntityName = "Authentication",
                    Timestamp = DateTime.UtcNow,
                    Details = "User logged in successfully."
                });

                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This account is temporarily locked.");
            }
            else
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");
            }

            return View(model);
        }

        // ----------------------------------------------------
        // LOGOUT
        // ----------------------------------------------------

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name ?? "Unknown";

            _context.AuditLogs.Add(new AuditLog
            {
                UserName = username,
                Action = "Logout",
                EntityName = "Authentication",
                Timestamp = DateTime.UtcNow,
                Details = "User logged out successfully."
            });

            await _context.SaveChangesAsync();

            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return Content("Access denied.");
        }
    }
}
