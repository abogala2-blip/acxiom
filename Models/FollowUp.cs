using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class FollowUp
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public DateTime FollowUpDate { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        public int? CustomerId { get; set; }

        public int? LeadId { get; set; }

        public int? OpportunityId { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Customer? Customer { get; set; }

        public Lead? Lead { get; set; }

        public Opportunity? Opportunity { get; set; }
    }
}