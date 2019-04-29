using System;
using Microsoft.AspNetCore.Identity;

namespace XipeADNWeb.Entities
{
    public class KPI
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }

        public Int32 OpportunityId { get; set; }
        public virtual Opportunity Opportunity { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }
    }
}

