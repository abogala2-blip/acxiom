namespace AcxiomCRM.Models
{
    public class UserManagementViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Roles { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
