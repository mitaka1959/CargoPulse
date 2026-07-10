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
        public string Status { get; private set; } = "Active";
        public bool IsDeleted { get; private set; } = false;

        private Driver() { }

        public Driver(string name, string licenceNumber, string status)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(licenceNumber))
                throw new ArgumentException("Licence number cannot be empty.", nameof(licenceNumber));
            
            Id = Guid.NewGuid();
            Name = name;
            LicenseNumber = licenceNumber;
            Status = status;
        }
    }
    enum DriverStatus
    {
        Active,
        Inactive,
        OnVacation,
        Suspended,
    }
}
