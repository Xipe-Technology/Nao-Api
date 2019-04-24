using System;
namespace XipeADNApp.Models
{
    public class UserModel : Model<string>
    {
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string Email { get; set; }
        public string FullName { get => string.IsNullOrEmpty(LastName) ? FirstName : $"{FirstName} {LastName}"; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Company { get; set; }
        public string CompanyRole { get; set; }
        public string Phone { get; set; }
        public string Card { get; set; }
    }
}
