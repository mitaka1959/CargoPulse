using CargoPulse.Fleet.Domain.Aggregates.DriverAggregates;
using CargoPulse.Fleet.Domain.Aggregates.VehicleAggregates;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates
{
    public class VehicleAssignment
    {
        public Guid Id { get; private set; }
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; }
        public DateTime AssignedAtUtc { get; private set; }
        public DateTime? ReleasedAtUtc { get; private set; }
        public AssignmentStatus Status { get; private set; }
        public Destination? StartAssigmentDestination { get; private set; }
        public Destination? EndAssigmentDestination { get; private set; }


        //nav properties
        public Vehicle Vehicle { get; private set; } = null!;
        public Driver Driver { get; private set; } = null!;

        private VehicleAssignment() { }

        public VehicleAssignment(Guid vehicleId, Guid driverId, DateTime assignedAtUtc, Destination? startAssigmentDestination = null, Destination? endAssigmentDestination = null)
        {
            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            DriverId = driverId;
            AssignedAtUtc = assignedAtUtc;
            StartAssigmentDestination = startAssigmentDestination;
            EndAssigmentDestination = endAssigmentDestination;

            Status = assignedAtUtc > DateTime.UtcNow
            ? AssignmentStatus.Scheduled
            : AssignmentStatus.Ongoing;
        }
        public void StartShift()
        {
            if (Status != AssignmentStatus.Scheduled)
                throw new InvalidOperationException("Only scheduled assignments can be started.");

            Status = AssignmentStatus.Ongoing;
        }

        public void ReleaseVehicle(DateTime releasedAtUtc)
        {
            if (Status == AssignmentStatus.Ended)
                throw new InvalidOperationException("This assignment has already ended.");
            if (releasedAtUtc < AssignedAtUtc)
                throw new ArgumentException("Release time cannot be before assignment time.");

            ReleasedAtUtc = releasedAtUtc;
            Status = AssignmentStatus.Ended;
        }
    }

    public record Destination(double Latitude, double Longitude);

    public enum AssignmentStatus
    {
        Scheduled = 1,
        Ongoing = 2,
        Ended = 3
    }
}
