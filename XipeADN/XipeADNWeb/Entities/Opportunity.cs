using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace XipeADNWeb.Entities
{
    public class Opportunity
    {
        public int Id { get; set; }
        public string Picture { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }
        public List<KPI> KPIs { get; set; }

        public String UserId { get; set; }
        public User User { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }
    }
}

