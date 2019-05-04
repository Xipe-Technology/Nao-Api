using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

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
        [JsonIgnore]
        public virtual List<Opportunity> Opportunitiesc { get; set; }

        //nuevo
        public string LinkedIn { get; set; }
        public string Twitter { get; set; }
        public string About { get; set; }
        public string Naos { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public Int32 Rank { get; set; }

        [NotMapped]
        public int? Opportunities { get => Opportunitiesc?.Count;}


    }
}

