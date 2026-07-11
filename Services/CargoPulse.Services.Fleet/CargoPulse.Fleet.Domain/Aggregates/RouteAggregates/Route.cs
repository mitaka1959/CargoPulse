using CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates;
using CargoPulse.Fleet.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.RouteAggregates
{
    public class Route : AggregateRoot
    {
        public string RouteName { get; private set; } = string.Empty;
        public double EstimatedDistanceKm { get; private set; }

        private readonly List<RouteStop> _stops = new();
        public IReadOnlyCollection<RouteStop> Stops => _stops.AsReadOnly();

        private Route() { }

        public Route(Guid id, string routeName, double estimatedDistanceKm)
        {
            if (string.IsNullOrWhiteSpace(routeName)) throw new ArgumentException("Route name required.", nameof(routeName));
            if (estimatedDistanceKm <= 0) throw new ArgumentException("Estimated distance must be positive.", nameof(estimatedDistanceKm));

            Id = id;
            RouteName = routeName;
            EstimatedDistanceKm = estimatedDistanceKm;
        }

        public RouteStop AddStop(int sequence, StopType stopType, GeoLocation location)
        {
            if (_stops.Any(s => s.StopSequence == sequence))
                throw new InvalidOperationException($"A stop with sequence {sequence} already exists.");

            var stop = new RouteStop(this.Id, sequence, stopType, location);
            _stops.Add(stop);
            return stop;
        }
    }
}
