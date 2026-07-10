using CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.VehicleAggregates
{
    public class Vehicle
    {
        public Guid Id { get; private set; }
        public string LicensePlate { get; private set; } = string.Empty;
        public VehicleStatus Status { get; private set; } = VehicleStatus.Available;
        public string Type { get; private set; } = "CargoTruck";
        public int MaxSpeedLimit { get; private set; }
        public double CapacityTonnage { get; private set; }
        public Destination CurrentLocation { get; private set; } = null!;
        public bool IsDeleted { get; private set; } = false;

        private Vehicle() { } 

        public Vehicle(Guid id, string licensePlate, string type, int maxSpeedLimit, double capacityTonnage, Destination currentLocation)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("License plate cannot be empty.", nameof(licensePlate));
            if (maxSpeedLimit <= 0)
                throw new ArgumentException("Max speed limit must be greater than zero.", nameof(maxSpeedLimit));
            if (capacityTonnage <= 0)
                throw new ArgumentException("Capacity tonnage must be greater than zero.", nameof(capacityTonnage));

            Id = id;
            LicensePlate = licensePlate;
            Type = type;
            MaxSpeedLimit = maxSpeedLimit;
            CapacityTonnage = capacityTonnage;
            CurrentLocation = currentLocation ?? throw new ArgumentNullException(nameof(currentLocation));
        }

        // State Transition Domain Behaviors
        public void AssignToRoute()
        {
            if (Status == VehicleStatus.Maintenance)
                throw new InvalidOperationException("Cannot assign a vehicle that is currently in maintenance.");

            Status = VehicleStatus.OnAssignment;
        }

        public void ReserveForUpcomingRoute()
        {
            if (Status != VehicleStatus.Available)
                throw new InvalidOperationException("Vehicle can only be reserved if it is currently available.");

            Status = VehicleStatus.Reserved;
        }

        public void CompleteRoute(Destination terminalLocation)
        {
            Status = VehicleStatus.Available;
            CurrentLocation = terminalLocation;
        }

        public void SendToMaintenance()
        {
            if (Status == VehicleStatus.OnAssignment)
                throw new InvalidOperationException("Cannot send a vehicle to maintenance while it is actively on a highway route.");

            Status = VehicleStatus.Maintenance;
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
