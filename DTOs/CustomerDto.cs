using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Company { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        public string? Status { get; set; }
    }
}
