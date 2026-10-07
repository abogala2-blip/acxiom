using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity;

namespace AcxiomCRM.Middleware
{
    public class SecurityAuditMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityAuditMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ApplicationDbContext db)
        {
            var path =
                context.Request.Path.Value ?? "";

            var method =
                context.Request.Method;

            var usernameBefore =
                context.User?.Identity?.Name;

            await _next(context);

            // -------------------------------------------------
            // Login success
            // -------------------------------------------------

            if (method == "POST" &&
                path.Contains("/Identity/Account/Login") &&
                context.Response.StatusCode >= 300 &&
                context.Response.StatusCode < 400)
            {
                var username =
                    context.User?.Identity?.Name
                    ?? usernameBefore
                    ?? "Unknown";

                db.AuditLogs.Add(new AuditLog
                {
                    UserName = username,
                    Action = "LoginSuccess",
                    EntityName = "Authentication",
                    Timestamp = DateTime.UtcNow,
                    Details = "Successful login."
                });

                await db.SaveChangesAsync();
            }

            // -------------------------------------------------
            // Login failure
            // -------------------------------------------------

            if (method == "POST" &&
                path.Contains("/Identity/Account/Login") &&
                context.Response.StatusCode == 200)
            {
                db.AuditLogs.Add(new AuditLog
                {
                    UserName = usernameBefore ?? "Unknown",
                    Action = "LoginFailure",
                    EntityName = "Authentication",
                    Timestamp = DateTime.UtcNow,
                    Details = "Login request failed or validation failed."
                });

                await db.SaveChangesAsync();
            }
        }
    }
}
