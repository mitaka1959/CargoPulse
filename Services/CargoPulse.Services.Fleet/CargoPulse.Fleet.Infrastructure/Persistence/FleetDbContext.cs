using CargoPulse.Fleet.Domain.Aggregates.AssigmentAggregates;
using CargoPulse.Fleet.Domain.Aggregates.DriverAggregates;
using CargoPulse.Fleet.Domain.Aggregates.RouteAggregates;
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
    public DbSet<Assignment> VehicleAssignments => Set<Assignment>();
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
            entity.Property(v => v.Status).HasConversion<string>().IsRequired().HasMaxLength(20); 
            entity.Property(v => v.Type).IsRequired().HasMaxLength(50);

            entity.HasQueryFilter(v => !v.IsDeleted); 

            
            entity.OwnsOne(v => v.CurrentLocation, loc =>
            {
                loc.Property(l => l.Latitude).HasColumnName("CurrentLatitude");
                loc.Property(l => l.Longitude).HasColumnName("CurrentLongitude");
                loc.Property(l => l.HubId).HasColumnName("CurrentHubId");
            });
        });

        // Corrected Driver Configuration
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.ToTable("Drivers");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            entity.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(50);
            entity.Property(d => d.Status).HasConversion<string>().IsRequired().HasMaxLength(20); 

            entity.HasQueryFilter(d => !d.IsDeleted); 

            
            entity.OwnsOne(d => d.HomeBaseLocation, loc =>
            {
                loc.Property(l => l.Latitude).HasColumnName("HomeLatitude");
                loc.Property(l => l.Longitude).HasColumnName("HomeLongitude");
                loc.Property(l => l.HubId).HasColumnName("HomeHubId");
            });
        });
      
        modelBuilder.Entity<Route>(entity =>
        {
            entity.ToTable("Routes");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.RouteName).IsRequired().HasMaxLength(150);
            entity.HasQueryFilter(r => !r.IsDeleted);

            entity.HasMany(r => r.Stops)
                  .WithOne()
                  .HasForeignKey(s => s.RouteId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Route Stop Mappings
        modelBuilder.Entity<RouteStop>(entity =>
        {
            entity.ToTable("RouteStops");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.StopType).IsRequired().HasMaxLength(50);

            entity.OwnsOne(s => s.Location, loc =>
            {
                loc.Property(l => l.Latitude).HasColumnName("Latitude");
                loc.Property(l => l.Longitude).HasColumnName("Longitude");
                loc.Property(l => l.HubId).HasColumnName("HubId");
            });
        });

        // New Assignment Mappings (Replaces VehicleAssignments)
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("Assignments");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Status).HasConversion<string>().IsRequired().HasMaxLength(20);
            entity.Property(a => a.CargoType).IsRequired().HasMaxLength(50);

            entity.Ignore(a => a.ProgressPercentage); 

            entity.HasOne(a => a.Vehicle).WithMany().HasForeignKey(a => a.VehicleId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(a => a.Driver).WithMany().HasForeignKey(a => a.DriverId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(a => a.Route).WithMany().HasForeignKey(a => a.RouteId).OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(a => a.Stops)
                  .WithOne()
                  .HasForeignKey(s => s.AssignmentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Assignment Stop Mappings (Live Execution)
        modelBuilder.Entity<AssignmentStop>(entity =>
        {
            entity.ToTable("AssignmentStops");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Status).HasConversion<string>().IsRequired().HasMaxLength(20);

            entity.HasOne<RouteStop>()
                  .WithMany()
                  .HasForeignKey(s => s.RouteStopId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Hub>(entity =>
        {
            entity.ToTable("Hubs");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(100);
            entity.Property(h => h.City).IsRequired().HasMaxLength(100);
            entity.Property(h => h.Country).IsRequired().HasMaxLength(100);

            
            entity.Ignore(h => h.FreeSpaceCapacity);
            entity.Ignore(h => h.TotalCapacity);

            entity.HasQueryFilter(h => !h.IsDeleted);

            // One-to-Many Relationship to Parking Spaces
            entity.HasMany(h => h.ParkingSpaces)
                  .WithOne()
                  .HasForeignKey(p => p.HubId)
                  .OnDelete(DeleteBehavior.Cascade);
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

