using System;
using Microsoft.AspNetCore.Identity;

namespace XipeADNWeb.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public string Reason { get; set; }

        public int OpportunityId { get; set; }
        public virtual Opportunity Opportunity { get; set; }

        public string UserId { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }
    }
}

