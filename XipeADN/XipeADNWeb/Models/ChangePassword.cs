using System;
namespace XipeADNWeb.Models
{
    public class ChangePassword
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string UserId { get; set; }
    }
}
