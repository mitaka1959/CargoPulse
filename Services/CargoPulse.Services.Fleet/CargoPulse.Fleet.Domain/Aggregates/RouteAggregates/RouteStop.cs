using CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates;
using CargoPulse.Fleet.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.RouteAggregates
{
    public class RouteStop : Entity
    {
        public Guid RouteId { get; private set; }

        public int StopSequence { get; private set; } 
        public StopType StopType { get; private set; }
        public GeoLocation Location { get; private set; } = null!;

        private RouteStop() { }

        internal RouteStop(Guid routeId, int stopSequence, StopType stopType, GeoLocation location)
        {
            if (stopSequence <= 0) throw new ArgumentException("Sequence must be positive.", nameof(stopSequence));

            Id = Guid.NewGuid();
            RouteId = routeId;
            StopSequence = stopSequence;
            StopType = stopType;
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }
    }

    public enum StopType
    {
        Origin = 1,
        Pickup = 2,
        Dropoff = 3,
        Destination = 4
    }
}