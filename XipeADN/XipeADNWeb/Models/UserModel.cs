using System;
using XipeADNWeb.Entities;
namespace XipeADNWeb.Models
{
    public class UserModel : Model<string>
    {
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Job { get; set; }
        public string Location { get; set; }
        public string Phone { get; set; }

        //new
        public string CountryCode { get; set; }
        public string CallNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string LinkedIn { get; set; }
        public string Twitter { get; set; }
        public string About { get; set; }
        public string Naos { get; set; }
        public string Rank { get; set; }
        public int? Opportunities { get; set; }
        public int? Matches { get; set; }

        public string ProfilePicUrl { get; set; }
        public string BannerPicUrl { get; set; }

    }
}
