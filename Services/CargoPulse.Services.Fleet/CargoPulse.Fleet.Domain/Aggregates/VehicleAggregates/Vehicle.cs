using CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates;
using CargoPulse.Fleet.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.VehicleAggregates
{
    public class Vehicle : AggregateRoot
    {
        public string LicensePlate { get; private set; } = string.Empty;
        public VehicleStatus Status { get; private set; } = VehicleStatus.Available;
        public VehicleType Type { get; private set; } = VehicleType.CargoTruck;
        public int MaxSpeedLimit { get; private set; }
        public double CapacityTonnage { get; private set; }
        public GeoLocation CurrentLocation { get; private set; } = null!;

        private Vehicle() { }

        public Vehicle(Guid id, string licensePlate, VehicleType type, int maxSpeedLimit, double capacityTonnage, GeoLocation currentLocation)
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
        public void ReserveForUpcomingRoute()
        {
            if (Status != VehicleStatus.Available)
                throw new InvalidOperationException("Vehicle can only be reserved if it is currently available.");

            Status = VehicleStatus.Reserved;
        }

        public void AssignToRoute()
        {
            if (Status is not (VehicleStatus.Available or VehicleStatus.Reserved))
                throw new InvalidOperationException($"Cannot assign a vehicle in status '{Status}'. It must be Available or Reserved.");

            Status = VehicleStatus.OnAssignment;
        }

        public void CompleteRoute(GeoLocation terminalLocation)
        {
            if (Status != VehicleStatus.OnAssignment)
                throw new InvalidOperationException("Only a vehicle that is currently on a route can complete it.");

            Status = VehicleStatus.Available;
            CurrentLocation = terminalLocation ?? throw new ArgumentNullException(nameof(terminalLocation));
        }

        public void SendToMaintenance()
        {
            if (Status == VehicleStatus.OnAssignment)
                throw new InvalidOperationException("Cannot send a vehicle to maintenance while it is actively on a highway route.");
            if (Status == VehicleStatus.Retired)
                throw new InvalidOperationException("A retired vehicle cannot be sent to maintenance.");

            Status = VehicleStatus.Maintenance;
        }

        public void ReturnToService()
        {
            if (Status != VehicleStatus.Maintenance)
                throw new InvalidOperationException("Only a vehicle currently in maintenance can be returned to service.");

            Status = VehicleStatus.Available;
        }

        public void Retire()
        {
            if (Status == VehicleStatus.OnAssignment)
                throw new InvalidOperationException("Cannot retire a vehicle while it is on assignment.");

            Status = VehicleStatus.Retired;
            Delete();
        }
    }
}
