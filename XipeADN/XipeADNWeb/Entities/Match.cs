using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XipeADNWeb.Entities
{
    public class Match
    {
        public int Id { get; set; }
        User User { get; set; }

        public Int32 OpportunityId { get; set; }
        public virtual Opportunity Opportunity { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
