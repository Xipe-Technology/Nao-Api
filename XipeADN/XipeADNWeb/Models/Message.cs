using System;
using System.ComponentModel.DataAnnotations;

namespace XipeADNWeb.Models
{
    public class Message
    {
        public string Body { get; set; }
        public string Subject { get; set; }
        public string Destination { get; set; }
    }
}
