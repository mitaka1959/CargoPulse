using CargoPulse.Fleet.Domain.Aggregates.VehicleAggregates;
using CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates;
using CargoPulse.Fleet.Domain.HubAggregates;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Infrastructure.Persistence.Seeder
{
    public class FleetDataSeeder
    {
        private readonly FleetDbContext _context;
        private readonly ILogger<FleetDataSeeder> _logger;

        public FleetDataSeeder(FleetDbContext context, ILogger<FleetDataSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            // 1. Check if we already have data. If we do, exit immediately.
            if (_context.Hubs.Any() || _context.Vehicles.Any())
            {
                _logger.LogInformation("Database already seeded. Skipping generation.");
                return;
            }

            _logger.LogInformation("Beginning massive Fleet Data Seeding...");

            // 2. Generate the European Hub Network
            var hubs = GenerateEuropeanHubs();
            await _context.Hubs.AddRangeAsync(hubs);

            // 3. Generate 100 Trucks distributed across these hubs
            var vehicles = GenerateTruckFleet(hubs);
            await _context.Vehicles.AddRangeAsync(vehicles);

            // 4. Save everything to PostgreSQL
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded {HubCount} Hubs and {VehicleCount} Vehicles!", hubs.Count, vehicles.Count);
        }

        private List<Hub> GenerateEuropeanHubs()
        {
            var hubs = new List<Hub>
        {
            new Hub(Guid.NewGuid(), "Berlin Central Node", "Berlin", "Germany", 52.5200, 13.4050),
            new Hub(Guid.NewGuid(), "Munich Logistics Center", "Munich", "Germany", 48.1351, 11.5820),
            new Hub(Guid.NewGuid(), "Rome Southern Hub", "Rome", "Italy", 41.9028, 12.4964),
            new Hub(Guid.NewGuid(), "Paris Gateway", "Paris", "France", 48.8566, 2.3522),
            new Hub(Guid.NewGuid(), "Madrid Transit Terminal", "Madrid", "Spain", 40.4168, -3.7038)
        };

            // Add 20 Parking Spaces to each Hub (Zones A and B)
            foreach (var hub in hubs)
            {
                for (int i = 1; i <= 10; i++) hub.AddParkingSpace("A", i);
                for (int i = 1; i <= 10; i++) hub.AddParkingSpace("B", i);
            }

            return hubs;
        }

        private List<Vehicle> GenerateTruckFleet(List<Hub> hubs)
        {
            var vehicles = new List<Vehicle>();
            var random = new Random();

            for (int i = 1; i <= 100; i++)
            {
                // Pick a random hub to spawn the truck at
                var homeHub = hubs[random.Next(hubs.Count)];
                var startLocation = new Destination(homeHub.Latitude, homeHub.Longitude, homeHub.Id);

                // Generate realistic logistics data
                string licensePlate = $"{homeHub.City.Substring(0, 1).ToUpper()}-TR-{1000 + i}";
                int speedLimit = 90; // Standard EU Truck Limit (km/h)
                double tonnage = random.Next(15, 40); // Between 15 and 40 tons
                string type = tonnage > 25 ? "HeavyFreight" : "StandardCargo";

                var truck = new Vehicle(Guid.NewGuid(), licensePlate, type, speedLimit, tonnage, startLocation);

                // Note: We leave the status as 'Available' natively!
                vehicles.Add(truck);
            }

            return vehicles;
        }
    }
}