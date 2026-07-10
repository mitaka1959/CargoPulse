using CargoPulse.Fleet.Domain.Aggregates.DriverAggregates;
using CargoPulse.Fleet.Domain.Aggregates.VehicleAggregates;
using CargoPulse.Fleet.Domain.Aggregates.RouteAggregates;   
using CargoPulse.Fleet.Domain.Aggregates.AssigmentAggregates;    
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates
{
    public class Assignment
    {
        public Guid Id { get; private set; }
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; }
        public Guid RouteId { get; private set; } // 🚀 Replaced all Destinations with a Route Link!

        public DateTime AssignedAtUtc { get; private set; }
        public DateTime? ReleasedAtUtc { get; private set; }

        // Cargo Details remain because they change per trip, even on the same route
        public string CargoType { get; private set; } = "General";
        public double CargoTonnage { get; private set; }
        public AssignmentStatus Status { get; private set; }

        // Live Execution Tracking
        private readonly List<AssignmentStop> _stops = new();
        public IReadOnlyCollection<AssignmentStop> Stops => _stops.AsReadOnly();

        // Navigation Links
        public Vehicle Vehicle { get; private set; } = null!;
        public Driver Driver { get; private set; } = null!;
        public Route Route { get; private set; } = null!; // Navigation to the Route Plan

        public double ProgressPercentage
        {
            get
            {
                if (!_stops.Any()) return 0.0;
                double completedStops = _stops.Count(s => s.Status == StopStatus.Completed);
                return (completedStops / _stops.Count) * 100;
            }
        }

        private Assignment() { }

        public Assignment(Guid vehicleId, Guid driverId, Guid routeId, DateTime assignedAtUtc, string cargoType, double cargoTonnage)
        {
            if (cargoTonnage <= 0) throw new ArgumentException("Cargo tonnage metric must be positive.");

            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            DriverId = driverId;
            RouteId = routeId;
            AssignedAtUtc = assignedAtUtc;
            CargoType = cargoType;
            CargoTonnage = cargoTonnage;

            Status = assignedAtUtc > DateTime.UtcNow
                ? AssignmentStatus.Scheduled
                : AssignmentStatus.Ongoing;
        }

        public void InitializeItinerary(IEnumerable<AssignmentStop> plannedStops)
        {
            if (_stops.Any()) throw new InvalidOperationException("Itinerary is already initialized.");
            _stops.AddRange(plannedStops);
        }

        public void RecordStopArrival(Guid assignmentStopId, DateTime arrivalTime)
        {
            var stop = _stops.FirstOrDefault(s => s.Id == assignmentStopId)
                       ?? throw new ArgumentException("Stop not found on this assignment.");

            stop.MarkArrived(arrivalTime);
        }

        public void CompleteAssignment(DateTime releasedAtUtc)
        {
            if (releasedAtUtc < AssignedAtUtc)
                throw new ArgumentException("Release time bounds violation.");

            ReleasedAtUtc = releasedAtUtc;
            Status = AssignmentStatus.Ended;
        }
    }
}
