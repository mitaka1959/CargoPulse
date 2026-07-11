using CargoPulse.Fleet.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates
{
    public class AssignmentStop : Entity
    {
        public Guid AssignmentId { get; private set; }
        public Guid RouteStopId { get; private set; }

        // Live Execution Metrics
        public DateTime EstimatedArrivalTimeUtc { get; private set; }
        public DateTime? ActualArrivalTimeUtc { get; private set; }

        public int ScheduledStayDurationMinutes { get; private set; }
        public DateTime? ActualDepartureTimeUtc { get; private set; }

        public StopStatus Status { get; private set; } = StopStatus.Pending;

        private AssignmentStop() { }

        internal AssignmentStop(Guid assignmentId, Guid routeStopId, DateTime eta, int scheduledStayDurationMinutes)
        {
            if (scheduledStayDurationMinutes < 0)
                throw new ArgumentException("Scheduled stay duration cannot be negative.", nameof(scheduledStayDurationMinutes));

            Id = Guid.NewGuid();
            AssignmentId = assignmentId;
            RouteStopId = routeStopId;
            EstimatedArrivalTimeUtc = eta;
            ScheduledStayDurationMinutes = scheduledStayDurationMinutes;
        }

        internal void MarkArrived(DateTime actualArrivalUtc)
        {
            if (Status != StopStatus.Pending) throw new InvalidOperationException("Only a pending stop can be marked as arrived.");
            ActualArrivalTimeUtc = actualArrivalUtc;
            Status = StopStatus.Arrived;
        }

        internal void MarkDeparted(DateTime actualDepartureUtc)
        {
            if (Status != StopStatus.Arrived) throw new InvalidOperationException("Must arrive before departing.");
            if (actualDepartureUtc < ActualArrivalTimeUtc) throw new ArgumentException("Cannot depart before arriving.", nameof(actualDepartureUtc));

            ActualDepartureTimeUtc = actualDepartureUtc;
            Status = StopStatus.Completed;
        }

        internal void MarkSkipped()
        {
            if (Status != StopStatus.Pending) throw new InvalidOperationException("Only a pending stop can be skipped.");
            Status = StopStatus.Skipped;
        }
    }
    public enum StopStatus { Pending = 1, Arrived = 2, Completed = 3, Skipped = 4 }
}
