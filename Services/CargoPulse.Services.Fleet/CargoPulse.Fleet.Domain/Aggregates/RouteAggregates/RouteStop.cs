using CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.RouteAggregates
{
    public class RouteStop
    {
        public Guid Id { get; private set; }
        public Guid RouteId { get; private set; }

        public int StopSequence { get; private set; } // e.g., 1 (Start), 2 (Pickup), 3 (End)
        public string StopType { get; private set; } // "Origin", "Pickup", "Dropoff", "Destination"
        public Destination Location { get; private set; } = null!;

        private RouteStop() { } 

        internal RouteStop(Guid routeId, int stopSequence, string stopType, Destination location)
        {
            if (stopSequence <= 0) throw new ArgumentException("Sequence must be positive.");

            Id = Guid.NewGuid();
            RouteId = routeId;
            StopSequence = stopSequence;
            StopType = stopType;
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }
    }
}