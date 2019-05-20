﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XipeADNWeb.Entities;

namespace XipeADNWeb.Data
{
    public class XipeADNDbContext : IdentityDbContext<User>
    {

        public DbSet<KPI> KPIs { get; set; }
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<Chat> Chat { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Match> Matches { get; set; }

        public XipeADNDbContext(DbContextOptions<XipeADNDbContext> options)
            : base(options)
        {
        }
    }
}
