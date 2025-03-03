using System.ComponentModel.DataAnnotations;

namespace ModelClasses.ViewModel
{
    public class RegisterViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public string StatusMessage { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password Did't Match")]
        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
