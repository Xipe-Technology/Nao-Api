using System;
using System.ComponentModel.DataAnnotations;

namespace XipeADNWeb.Models
{
    public class RegisterModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        public string FullName { get => string.IsNullOrEmpty(LastName) ? FirstName : $"{FirstName} {LastName}"; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
