using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.VehicleAggregates
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string LicensePlate { get; private set; } = string.Empty;
        public string Status { get; private set; } = "Active";
        public string Type { get; private set; } = "CargoTruck";
        public int MaxSpeedLimit { get; private set; }
        public double Capacity { get; private set; }
        public bool IsDeleted { get; private set; } = false;
        private Vehicle() { }

        public Vehicle(string licensePlate, string status, string type, int maxSpeedLimit, double capacity)
        {

            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("License plate cannot be empty.", nameof(licensePlate));
            if (maxSpeedLimit <= 0)
                throw new ArgumentException("Max speed limit must be greater than zero.", nameof(maxSpeedLimit));
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));

            Id = Guid.NewGuid();
            LicensePlate = licensePlate;
            Status = status;
            Type = type;
            MaxSpeedLimit = maxSpeedLimit;
            Capacity = capacity;
        }
    }
}
