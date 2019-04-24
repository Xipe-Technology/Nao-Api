using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XipeADNWeb.Entities;

namespace XipeADNWeb.Data
{
    public class XipeADNDbContext : IdentityDbContext<User>
    {
        public XipeADNDbContext(DbContextOptions<XipeADNDbContext> options)
            : base(options)
        {
        }
    }
}
