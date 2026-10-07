using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class CreateUserViewModel
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /*
         * Assignment/demo requirement:
         * any non-empty password is accepted.
         */
        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "SalesExecutive";
    }
}
