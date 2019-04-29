using System;
using Microsoft.AspNetCore.Identity;

namespace XipeADNWeb.Entities
{
    public class Lead
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Agent { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }
    }
}

