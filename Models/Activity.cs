using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string ActivityType { get; set; } = "Call";

        [Required]
        public DateTime ActivityDate { get; set; } = DateTime.Now;

        [StringLength(30)]
        public string Status { get; set; } = "Planned";

        public int? CustomerId { get; set; }

        public int? LeadId { get; set; }

        public int? OpportunityId { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public Customer? Customer { get; set; }

        public Lead? Lead { get; set; }

        public Opportunity? Opportunity { get; set; }
    }
}