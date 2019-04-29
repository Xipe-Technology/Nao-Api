using System;
using Microsoft.AspNetCore.Identity;

namespace XipeADNWeb.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public string Job { get; set; }
        public string Location { get; set; }
        public string ProfilePicUrl { get; set; }
        public string BannerPicUrl { get; set; }

        //nuevo
        public string LinkedIn { get; set; }
        public string Twitter { get; set; }
        public string About { get; set; }
        public string Naos { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }
    }
}

