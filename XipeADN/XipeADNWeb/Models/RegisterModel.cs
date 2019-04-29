﻿using System;
using System.ComponentModel.DataAnnotations;

namespace XipeADNWeb.Models
{
    public class RegisterModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
