using CargoPulse.Fleet.Domain.Aggregates.DriverAggregates;
using CargoPulse.Fleet.Domain.Aggregates.VehicleAggregates;
using CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates;
using CargoPulse.Fleet.Domain.HubAggregates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CargoPulse.Fleet.Infrastructure.Persistence;

public class FleetDbContext : DbContext
{
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<VehicleAssignment> VehicleAssignments => Set<VehicleAssignment>();
    public DbSet<Hub> Hubs => Set<Hub>();

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

        modelBuilder.Entity<Hub>(entity =>
        {
            entity.ToTable("Hubs");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(100);
            entity.Property(h => h.City).IsRequired().HasMaxLength(100);
            entity.Property(h => h.Country).IsRequired().HasMaxLength(100);

            // We do NOT map FreeSpaceCapacity or TotalCapacity because they are computed!
            entity.Ignore(h => h.FreeSpaceCapacity);
            entity.Ignore(h => h.TotalCapacity);

            entity.HasQueryFilter(h => !h.IsDeleted);

            // One-to-Many Relationship to Parking Spaces
            entity.HasMany(h => h.ParkingSpaces)
                  .WithOne()
                  .HasForeignKey(p => p.HubId)
                  .OnDelete(DeleteBehavior.Cascade); // If a Hub is destroyed, its spaces are destroyed
        });

        // Parking Space Mappings
        modelBuilder.Entity<ParkingSpace>(entity =>
        {
            entity.ToTable("ParkingSpaces");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.SpaceDesignation).IsRequired().HasMaxLength(50);

            // Foreign Key to the Vehicle occupying it
            entity.HasOne<Vehicle>()
                  .WithMany()
                  .HasForeignKey(p => p.OccupiedByVehicleId)
                  .OnDelete(DeleteBehavior.SetNull); // If vehicle is deleted, space becomes empty
        });
    }
}

