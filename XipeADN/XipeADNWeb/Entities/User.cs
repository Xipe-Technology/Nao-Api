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
        //public string Password { get; set; }
        public string CountryCode { get; set; }
        public string CallNumber { get; set; }
        public string LinkedIn { get; set; }
        public string Twitter { get; set; }
        public string About { get; set; }
        public string Naos { get; set; }
        //public List<SocialMedia> Social { get; set; }
        //public List<GalleryItem> Gallery { get; set; }
        public string Token { get; set; }
        public string TokenExpiration { get; set; }


        [JsonIgnore]
        public virtual List<Opportunity> Opportunitiesc { get; set; }
        [JsonIgnore]
        public virtual List<Match> Matchesc { get; set; }


        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public int Rank { get; set; }
        [NotMapped]
        public int? Opportunities { get => Opportunitiesc?.Count;}
        [NotMapped]
        public int? Matches { get => Matchesc?.Count; }


    }
}

