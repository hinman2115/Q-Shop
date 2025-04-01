using System.ComponentModel.DataAnnotations;

namespace QShop.Models.ViewModel.Login
{
    public class RegisterViewModel
    {
        //

        [Required, Display(Name = "Full Name")]
        public string Name { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, Phone]
        public string Phone { get; set; } = null!;

        [Required]
        public int AreaId { get; set; }

        public string? Address { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]  // Ensure role is always provided
        public string Role { get; set; } = "Customer";
        //
    }
}





