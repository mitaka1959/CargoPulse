using CargoPulse.Fleet.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.HubAggregates
{
    public class ParkingSpace : Entity
    {
        public Guid HubId { get; private set; }
        public string SpaceDesignation { get; private set; } = string.Empty;

        public Guid? OccupiedByVehicleId { get; private set; }
        public DateTime? OccupiedFromUtc { get; private set; }
        public DateTime? OccupiedUntilUtc { get; private set; }

        public bool IsBooked => OccupiedByVehicleId.HasValue;

        private ParkingSpace() { }

        internal ParkingSpace(Guid hubId, string spaceDesignation)
        {
            if (string.IsNullOrWhiteSpace(spaceDesignation))
                throw new ArgumentException("Space designation is required.", nameof(spaceDesignation));

            Id = Guid.NewGuid();
            HubId = hubId;
            SpaceDesignation = spaceDesignation;
        }

        public bool IsOccupiedAt(DateTime instantUtc) =>
            OccupiedByVehicleId.HasValue
            && OccupiedFromUtc <= instantUtc
            && instantUtc < OccupiedUntilUtc;

        internal void Occupy(Guid vehicleId, DateTime fromUtc, DateTime untilUtc)
        {
            if (vehicleId == Guid.Empty)
                throw new ArgumentException("Vehicle id is required.", nameof(vehicleId));
            if (fromUtc >= untilUtc)
                throw new ArgumentException("Occupancy start time must be before end time.", nameof(fromUtc));

            if (Overlaps(fromUtc, untilUtc))
                throw new InvalidOperationException($"Space {SpaceDesignation} is already booked until {OccupiedUntilUtc:u}.");

            OccupiedByVehicleId = vehicleId;
            OccupiedFromUtc = fromUtc;
            OccupiedUntilUtc = untilUtc;
        }

        internal void Vacate()
        {
            if (!OccupiedByVehicleId.HasValue)
                throw new InvalidOperationException($"Space {SpaceDesignation} is not occupied.");

            OccupiedByVehicleId = null;
            OccupiedFromUtc = null;
            OccupiedUntilUtc = null;
        }

        private bool Overlaps(DateTime fromUtc, DateTime untilUtc) =>
            OccupiedFromUtc.HasValue && OccupiedUntilUtc.HasValue
            && fromUtc < OccupiedUntilUtc.Value
            && OccupiedFromUtc.Value < untilUtc;
    }
}
