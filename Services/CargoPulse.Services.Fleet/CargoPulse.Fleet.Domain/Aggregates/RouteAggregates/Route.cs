using CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.RouteAggregates
{
    public class Route
    {
        public Guid Id { get; private set; }
        public string RouteName { get; private set; } = string.Empty;
        public double EstimatedDistanceKm { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        private readonly List<RouteStop> _stops = new();
        public IReadOnlyCollection<RouteStop> Stops => _stops.AsReadOnly();

        private Route() { }

        public Route(Guid id, string routeName, double estimatedDistanceKm)
        {
            if (string.IsNullOrWhiteSpace(routeName)) throw new ArgumentException("Route name required.");

            Id = id;
            RouteName = routeName;
            EstimatedDistanceKm = estimatedDistanceKm;
        }

        public void AddStop(int sequence, string stopType, Destination location)
        {
            if (_stops.Any(s => s.StopSequence == sequence))
                throw new InvalidOperationException($"A stop with sequence {sequence} already exists.");

            _stops.Add(new RouteStop(this.Id, sequence, stopType, location));
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
