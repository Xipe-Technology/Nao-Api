using System;
using Microsoft.AspNetCore.Identity;

namespace XipeADNWeb.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string CompanyRole { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastEditDate { get; set; }
        public bool IsDeleted { get; set; }
        public string ProfilePicUrl { get; set; }
    }
}

//xipe, sofom, patron, usuario