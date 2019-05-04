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

        public string ProfilePicUrl { get; set; }
        public string BannerPicUrl { get; set; }

    }
}
