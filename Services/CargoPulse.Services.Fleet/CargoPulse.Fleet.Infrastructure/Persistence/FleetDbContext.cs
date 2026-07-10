using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CargoPulse.Fleet.Domain.Aggregates.VehicleAggregates;
using CargoPulse.Fleet.Domain.Aggregates.DriverAggregates;
using CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates;


namespace CargoPulse.Fleet.Infrastructure.Persistence;

public class FleetDbContext : DbContext
{
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<VehicleAssignment> VehicleAssignments => Set<VehicleAssignment>();

    public FleetDbContext(DbContextOptions<FleetDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Vehicle Configuration
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("Vehicles");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.LicensePlate).IsRequired().HasMaxLength(20);
            entity.Property(v => v.Status).IsRequired().HasMaxLength(30);
            entity.Property(v => v.Type).IsRequired().HasMaxLength(50);
        });

        // Driver Configuration
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.ToTable("Drivers");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            entity.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(50);
            entity.Property(d => d.Status).IsRequired().HasMaxLength(30);
        });

        // VehicleAssignment Configuration
        modelBuilder.Entity<VehicleAssignment>(entity =>
        {
            entity.ToTable("VehicleAssignments");
            entity.HasKey(va => va.Id);

            // Mapping the Destination Value Object as Owned Types
            entity.OwnsOne(va => va.StartAssigmentDestination, dest =>
            {
                dest.Property(d => d.Latitude).HasColumnName("StartLatitude");
                dest.Property(d => d.Longitude).HasColumnName("StartLongitude");
            });

            entity.OwnsOne(va => va.EndAssigmentDestination, dest =>
            {
                dest.Property(d => d.Latitude).HasColumnName("EndLatitude");
                dest.Property(d => d.Longitude).HasColumnName("EndLongitude");
            });

            // Relationships
            entity.HasOne(va => va.Vehicle)
                  .WithMany()
                  .HasForeignKey(va => va.VehicleId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(va => va.Driver)
                  .WithMany()
                  .HasForeignKey(va => va.DriverId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Property(va => va.Status)
                  .HasConversion<string>()
                  .IsRequired()
                  .HasMaxLength(20);
        });
    }
}

