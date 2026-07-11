using CargoPulse.Fleet.Domain.Aggregates.AssigmentAggregates;    
using CargoPulse.Fleet.Domain.Aggregates.DriverAggregates;
using CargoPulse.Fleet.Domain.Aggregates.RouteAggregates;   
using CargoPulse.Fleet.Domain.Aggregates.VehicleAggregates;
using CargoPulse.Fleet.Domain.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates
{
    public class Assignment : Entity
    {
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; }
        public Guid RouteId { get; private set; }

        public DateTime AssignedAtUtc { get; private set; }
        public DateTime? ReleasedAtUtc { get; private set; }

        public string CargoType { get; private set; } = "General";
        public double CargoTonnage { get; private set; }
        public AssignmentStatus Status { get; private set; }

        // Live Execution Tracking
        private readonly List<AssignmentStop> _stops = new();
        public IReadOnlyCollection<AssignmentStop> Stops => _stops.AsReadOnly();

        public double ProgressPercentage
        {
            get
            {
                if (_stops.Count == 0) return 0.0;
                int resolved = _stops.Count(s => s.Status is StopStatus.Completed or StopStatus.Skipped);
                return (double)resolved / _stops.Count * 100;
            }
        }

        private Assignment() { }

        public Assignment(Guid vehicleId, Guid driverId, Guid routeId, DateTime assignedAtUtc, string cargoType, double cargoTonnage)
        {
            if (vehicleId == Guid.Empty) throw new ArgumentException("Vehicle id is required.", nameof(vehicleId));
            if (driverId == Guid.Empty) throw new ArgumentException("Driver id is required.", nameof(driverId));
            if (routeId == Guid.Empty) throw new ArgumentException("Route id is required.", nameof(routeId));
            if (string.IsNullOrWhiteSpace(cargoType)) throw new ArgumentException("Cargo type is required.", nameof(cargoType));
            if (cargoTonnage <= 0) throw new ArgumentException("Cargo tonnage metric must be positive.", nameof(cargoTonnage));

            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            DriverId = driverId;
            RouteId = routeId;
            AssignedAtUtc = NormalizeToUtc(assignedAtUtc);
            CargoType = cargoType;
            CargoTonnage = cargoTonnage;

            Status = AssignedAtUtc > DateTime.UtcNow
                ? AssignmentStatus.Scheduled
                : AssignmentStatus.Ongoing;
        }

        public void InitializeItinerary(IEnumerable<PlannedStop> plannedStops)
        {
            if (plannedStops is null) throw new ArgumentNullException(nameof(plannedStops));
            if (_stops.Count > 0) throw new InvalidOperationException("Itinerary is already initialized.");

            foreach (var planned in plannedStops)
            {
                _stops.Add(new AssignmentStop(
                    Id,
                    planned.RouteStopId,
                    NormalizeToUtc(planned.EstimatedArrivalUtc),
                    planned.ScheduledStayDurationMinutes));
            }

            if (_stops.Count == 0)
                throw new ArgumentException("An itinerary must contain at least one stop.", nameof(plannedStops));
        }

        public void Begin()
        {
            if (Status != AssignmentStatus.Scheduled)
                throw new InvalidOperationException("Only a scheduled assignment can be started.");

            Status = AssignmentStatus.Ongoing;
        }

        public void RecordStopArrival(Guid assignmentStopId, DateTime arrivalTimeUtc)
        {
            EnsureOngoing();
            GetStop(assignmentStopId).MarkArrived(NormalizeToUtc(arrivalTimeUtc));
        }

        public void RecordStopDeparture(Guid assignmentStopId, DateTime departureTimeUtc)
        {
            EnsureOngoing();
            GetStop(assignmentStopId).MarkDeparted(NormalizeToUtc(departureTimeUtc));
        }

        public void SkipStop(Guid assignmentStopId)
        {
            EnsureOngoing();
            GetStop(assignmentStopId).MarkSkipped();
        }

        public void CompleteAssignment(DateTime releasedAtUtc)
        {
            if (Status is AssignmentStatus.Completed or AssignmentStatus.Cancelled)
                throw new InvalidOperationException($"An assignment that is {Status} cannot be completed.");

            releasedAtUtc = NormalizeToUtc(releasedAtUtc);
            if (releasedAtUtc < AssignedAtUtc)
                throw new ArgumentException("Release time cannot precede the assignment time.", nameof(releasedAtUtc));

            ReleasedAtUtc = releasedAtUtc;
            Status = AssignmentStatus.Completed;
        }

        public void Cancel()
        {
            if (Status is AssignmentStatus.Completed or AssignmentStatus.Cancelled)
                throw new InvalidOperationException($"An assignment that is {Status} cannot be cancelled.");

            Status = AssignmentStatus.Cancelled;
        }

        private AssignmentStop GetStop(Guid assignmentStopId) =>
            _stops.FirstOrDefault(s => s.Id == assignmentStopId)
            ?? throw new ArgumentException("Stop not found on this assignment.", nameof(assignmentStopId));

        private void EnsureOngoing()
        {
            if (Status != AssignmentStatus.Ongoing)
                throw new InvalidOperationException($"Stops can only be updated while the assignment is ongoing (current: {Status}).");
        }

        private static DateTime NormalizeToUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
    public readonly record struct PlannedStop(Guid RouteStopId, DateTime EstimatedArrivalUtc, int ScheduledStayDurationMinutes);
}
    
