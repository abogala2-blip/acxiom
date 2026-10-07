using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.DTOs
{
    public class OpportunityDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string OpportunityName { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }

        public int? LeadId { get; set; }

        [Range(0.01, 1000000000)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string Stage { get; set; } = "Qualification";

        [Range(0, 100)]
        public int Probability { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpectedCloseDate { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Open";

        public string? AssignedTo { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
