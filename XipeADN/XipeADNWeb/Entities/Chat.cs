using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace XipeADNWeb.Entities
{
    public class Chat
    {
        public string Id { get; set; }
        public virtual List<Message> Messages { get; set; }
        public User User1 { get; set; }
        public User User2 { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }
    }
}

