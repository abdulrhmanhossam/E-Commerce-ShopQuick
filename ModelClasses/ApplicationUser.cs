using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ModelClasses
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? State { get; set; }
    }
}
