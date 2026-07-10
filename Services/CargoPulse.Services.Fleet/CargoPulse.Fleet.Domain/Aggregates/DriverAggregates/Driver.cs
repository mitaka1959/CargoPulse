using CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.DriverAggregates
{
    public class Driver
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string LicenseNumber { get; private set; } = string.Empty;
        public DriverStatus Status { get; private set; }
        public Destination HomeBaseLocation { get; private set; } = null!;
        public bool IsDeleted { get; private set; } = false;

        private Driver() { } 

        public Driver(Guid id, string name, string licenseNumber, Destination homeBaseLocation)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Driver name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(licenseNumber))
                throw new ArgumentException("License number cannot be empty.", nameof(licenseNumber));

            Id = id;
            Name = name;
            LicenseNumber = licenseNumber;
            HomeBaseLocation = homeBaseLocation ?? throw new ArgumentNullException(nameof(homeBaseLocation));
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
