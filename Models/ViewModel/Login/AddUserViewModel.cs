using System.ComponentModel.DataAnnotations;

namespace QShop.Models.ViewModel.Login
{
    public class AddUserViewModel
    {

        [Required, Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;

        public string? Address { get; set; } = string.Empty;

        
        public int AreaId { get; set; } 

        [Required]
        public string Role { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
