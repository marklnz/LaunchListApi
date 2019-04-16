using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchListApi.Model.DataAccess
{
    public class LaunchListApiContext: DbContext
    {
        // Domain (Query) Model
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }

        // Domain Event dbset
        public DbSet<DomainEvent> DomainEvents { get; set; }
        
        // Audit Log dbset
        public DbSet<AuditLogEntry> AuditLog { get; set; }

        public LaunchListApiContext()
        {
        }

        public LaunchListApiContext(DbContextOptions<LaunchListApiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Declare value conversions (for Enums mostly - so we can save them as their string representation rather than as the underlying value, which is meaningless in the database)
            //modelBuilder.Entity<Agency>().Property(e => e.Status).HasConversion<string>();
            //modelBuilder.Entity<Card>().Property(e => e.Status).HasConversion<string>();
            //modelBuilder.Entity<Client>().Property(e => e.DisabilityType).HasConversion<string>();
            //modelBuilder.Entity<Client>().Property(e => e.Entitlement).HasConversion<string>();
            //modelBuilder.Entity<Client>().Property(e => e.Status).HasConversion<string>();
            //modelBuilder.Entity<Client>().Property(e => e.ContactVia).HasConversion<string>();
            //modelBuilder.Entity<Tenant>().Property(e => e.Status).HasConversion<string>();
            //modelBuilder.Entity<TransportOperator>().Property(e => e.Status).HasConversion<string>();
            //modelBuilder.Entity<Driver>().Property(e => e.Status).HasConversion<string>();
            //modelBuilder.Entity<Vehicle>().Property(e => e.Status).HasConversion<string>();

            modelBuilder.Entity<DomainEvent>().Property(e => e.DomainEventType).HasConversion<string>();

            modelBuilder.Entity<AuditLogEntry>().Property(e => e.DomainEventType).HasConversion<string>();
            modelBuilder.Entity<AuditLogEntry>().Property(e => e.AuditType).HasConversion<string>();
            modelBuilder.Entity<AuditLogEntry>().Property(e => e.DataAccessType).HasConversion<string>();

            // Default values and Store generated value fields (except identity columns)
            //modelBuilder.Entity<Client>().Property(e => e.ContactVia).HasDefaultValue(ContactVia.Mail);

            // Set NOT NULL on certain fields that we're allowing to be null initially and in DTOs, but which are NOT nullable in the database
            //modelBuilder.Entity<Agency>().Property(d => d.Status).IsRequired();
            //modelBuilder.Entity<Client>().Property(d => d.Status).IsRequired();
            //modelBuilder.Entity<Driver>().Property(d => d.Status).IsRequired();
            //modelBuilder.Entity<TransportOperator>().Property(d => d.Status).IsRequired();
            //modelBuilder.Entity<Tenant>().Property(d => d.Status).IsRequired();
            //modelBuilder.Entity<Vehicle>().Property(d => d.Status).IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
#endif
        }
    }
}
