using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.AssigmentAggregates
{
    public class AssignmentStop
    {
        public Guid Id { get; private set; }
        public Guid AssignmentId { get; private set; }
        public Guid RouteStopId { get; private set; } // Links back to the static Route Template

        // Live Execution Metrics
        public DateTime EstimatedArrivalTimeUtc { get; private set; }
        public DateTime? ActualArrivalTimeUtc { get; private set; }

        public int ScheduledStayDurationMinutes { get; private set; }
        public DateTime? ActualDepartureTimeUtc { get; private set; }

        public StopStatus Status { get; private set; } = StopStatus.Pending;

        private AssignmentStop() { }

        internal AssignmentStop(Guid assignmentId, Guid routeStopId, DateTime eta, int scheduledStayDurationMinutes)
        {
            Id = Guid.NewGuid();
            AssignmentId = assignmentId;
            RouteStopId = routeStopId;
            EstimatedArrivalTimeUtc = eta;
            ScheduledStayDurationMinutes = scheduledStayDurationMinutes;
        }

        // Domain Behaviors
        public void MarkArrived(DateTime actualArrivalUtc)
        {
            if (Status != StopStatus.Pending) throw new InvalidOperationException("Stop is already processed.");
            ActualArrivalTimeUtc = actualArrivalUtc;
            Status = StopStatus.Arrived;
        }

        public void MarkDeparted(DateTime actualDepartureUtc)
        {
            if (Status != StopStatus.Arrived) throw new InvalidOperationException("Must arrive before departing.");
            if (actualDepartureUtc < ActualArrivalTimeUtc) throw new ArgumentException("Cannot depart before arriving.");

            ActualDepartureTimeUtc = actualDepartureUtc;
            Status = StopStatus.Completed;
        }
    }

    public enum StopStatus { Pending = 1, Arrived = 2, Completed = 3, Skipped = 4 }
}
