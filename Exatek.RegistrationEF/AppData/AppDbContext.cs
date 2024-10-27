using Exatek.RegistrationCore.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.RegistrationEF.AppData;
public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Customer>(prop =>
        {
            prop.HasKey(p => p.ICNumber);
            prop.Property(p => p.ICNumber).ValueGeneratedNever();
            prop.Property(p => p.Name).IsRequired().HasMaxLength(150);
            prop.Property(p => p.Email).IsRequired().HasMaxLength(150);
            prop.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(150);

        });
        base.OnModelCreating(builder);
    }
}
