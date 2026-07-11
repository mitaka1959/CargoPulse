using CargoPulse.Fleet.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.HubAggregates
{
    public class Hub : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        // The Hub manages its own spaces
        private readonly List<ParkingSpace> _parkingSpaces = new();
        public IReadOnlyCollection<ParkingSpace> ParkingSpaces => _parkingSpaces.AsReadOnly();

        public int TotalCapacity => _parkingSpaces.Count;

        private Hub() { }

        public Hub(Guid id, string name, string city, string country, double latitude, double longitude)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Hub name is required.", nameof(name));

            Id = id;
            Name = name;
            City = city;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
        }

        public int FreeSpaceCapacityAt(DateTime instantUtc) =>
            _parkingSpaces.Count(space => !space.IsOccupiedAt(instantUtc));

        public void AddParkingSpace(string zone, int number)
        {
            if (string.IsNullOrWhiteSpace(this.City) || this.City.Length < 3)
                throw new InvalidOperationException("Hub must have a valid City/IATA code.");
            if (string.IsNullOrWhiteSpace(zone))
                throw new ArgumentException("Zone is required.", nameof(zone));
            if (number <= 0)
                throw new ArgumentException("Space number must be positive.", nameof(number));

            string cityCode = this.City.Substring(0, 3).ToUpper();
            string designation = $"{cityCode}-{zone.ToUpper()}-{number:D2}";

            if (_parkingSpaces.Any(p => p.SpaceDesignation == designation))
                throw new InvalidOperationException($"Parking space {designation} already exists.");

            _parkingSpaces.Add(new ParkingSpace(this.Id, designation));
        }

        public void OccupySpace(string designation, Guid vehicleId, DateTime fromUtc, DateTime untilUtc)
        {
            GetSpace(designation).Occupy(vehicleId, fromUtc, untilUtc);
        }

        public void VacateSpace(string designation)
        {
            GetSpace(designation).Vacate();
        }

        private ParkingSpace GetSpace(string designation) =>
            _parkingSpaces.FirstOrDefault(p => p.SpaceDesignation == designation)
            ?? throw new ArgumentException($"Parking space {designation} does not exist at this hub.", nameof(designation));
    }
}
