using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace XipeADNWeb.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime MessageDateTime { get; set; }
        public bool IsIncoming { get; set; }
        public string AttachementUrl { get; set; }

        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }

        public virtual Chat Chat { get; set; }
        public int ChatId { get; set; }

    }
}

