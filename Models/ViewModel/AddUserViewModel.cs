using System.ComponentModel.DataAnnotations;

namespace QShop.Models.ViewModel
{
    public class AddUserViewModel
    {

        [Required, Display(Name = "Full Name")]
        public string Name { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, Phone]
        public string Phone { get; set; } = null!;

        public string? Address { get; set; }

        [Required]
        public int AreaId { get; set; }

        [Required]
        public string Role { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
