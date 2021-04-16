using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BankEmulatorDB
{
    public class BankEmulatorDbContext : DbContext
    {
        public BankEmulatorDbContext(DbContextOptions<BankEmulatorDbContext> options) : base(options) { }
    }
}
