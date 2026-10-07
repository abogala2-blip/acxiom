using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [StringLength(256)]
        public string? UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EntityName { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [StringLength(1000)]
        public string? Details { get; set; }
    }
}
