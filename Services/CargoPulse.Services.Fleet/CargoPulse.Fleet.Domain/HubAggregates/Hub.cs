using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.HubAggregates
{
    public class Hub
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string Country { get; private set; } = string.Empty;
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        // The Hub manages its own spaces 
        private readonly List<ParkingSpace> _parkingSpaces = new();
        public IReadOnlyCollection<ParkingSpace> ParkingSpaces => _parkingSpaces.AsReadOnly();

        
        public int FreeSpaceCapacity => _parkingSpaces.Count(space => !space.IsOccupied);
        public int TotalCapacity => _parkingSpaces.Count;

        private Hub() { }

        public Hub(Guid id, string name, string city, string country, double latitude, double longitude)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Hub name is required.");

            Id = id;
            Name = name;
            City = city;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
        }

        public void AddParkingSpace(string spaceDesignation)
        {
            if (_parkingSpaces.Any(p => p.SpaceDesignation == spaceDesignation))
                throw new InvalidOperationException("A parking space with this designation already exists.");

            _parkingSpaces.Add(new ParkingSpace(this.Id, spaceDesignation));
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
