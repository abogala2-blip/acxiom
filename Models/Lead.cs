using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class Lead
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string LeadName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? CompanyName { get; set; }

        [Required]
        public string Source { get; set; } = "Website";

        [Required]
        public string Status { get; set; } = "New";

        [Range(0, 100000000)]
        public decimal ExpectedValue { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? AssignedTo { get; set; }

        public string? Notes { get; set; }
    }
}