using AcxiomCRM.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Lead> Leads { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<FollowUp> FollowUps { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var auditEntries = ChangeTracker.Entries()
                .Where(e =>
                    e.Entity is Customer ||
                    e.Entity is Lead ||
                    e.Entity is Opportunity ||
                    e.Entity is FollowUp ||
                    e.Entity is Activity)
                .Where(e =>
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted)
                .Select(e => new
                {
                    Entry = e,
                    Action = e.State switch
                    {
                        EntityState.Added => "Create",
                        EntityState.Modified => "Update",
                        EntityState.Deleted => "Delete",
                        _ => "Unknown"
                    },
                    EntityName = e.Entity.GetType().Name
                })
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (auditEntries.Count > 0)
            {
                var userName =
                    _httpContextAccessor?.HttpContext?.User?.Identity?.Name
                    ?? "System";

                foreach (var item in auditEntries)
                {
                    int? entityId = null;

                    var idProperty = item.Entry.Properties
                        .FirstOrDefault(p => p.Metadata.Name == "Id");

                    if (idProperty != null && idProperty.CurrentValue != null)
                    {
                        entityId = Convert.ToInt32(idProperty.CurrentValue);
                    }

                    AuditLogs.Add(new AuditLog
                    {
                        UserName = userName,
                        Action = item.Action,
                        EntityName = item.EntityName,
                        EntityId = entityId,
                        Timestamp = DateTime.UtcNow,
                        Details =
                            $"{item.Action} operation performed on {item.EntityName}."
                    });
                }

                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
    }
}
