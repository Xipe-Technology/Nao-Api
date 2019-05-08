using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace XipeADNWeb.Entities
{
    public class Chat
    {
        public int Id { get; set; }
        public virtual List<Message> Messages { get; set; }
        public virtual User User1 { get; set; }
        public string User1Id { get; set; }
        public virtual User User2 { get; set; }
        public string User2Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped]
        public Message LastMessage { get; set; }
    }
}

