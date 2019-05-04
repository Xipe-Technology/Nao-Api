using System;
using System.ComponentModel.DataAnnotations;

namespace XipeADNWeb.Models
{
    public class ChangePassword
    {
        [Required]
        //[StringLength(18, ErrorMessage = "The current password must be at least {2} characters long and minimum one special character.", MinimumLength = 6)]
        //[RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        //[DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(18, ErrorMessage = "The new password must be at least {2} characters long and one special character minimum.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "The new password must be at least 6 characters long and one special character minimum.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        public string UserId { get; set; }
    }
}
