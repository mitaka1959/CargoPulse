using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.HubAggregates
{
    public class ParkingSpace
    {
        public Guid Id { get; private set; }
        public Guid HubId { get; private set; }
        public string SpaceDesignation { get; private set; }

        // Occupancy Tracking
        public Guid? OccupiedByVehicleId { get; private set; }
        public DateTime? OccupiedFromUtc { get; private set; }
        public DateTime? OccupiedUntilUtc { get; private set; }

        // Computed shortcut
        public bool IsOccupied => OccupiedByVehicleId.HasValue;

        private ParkingSpace() { }

        internal ParkingSpace(Guid hubId, string spaceDesignation)
        {
            if (string.IsNullOrWhiteSpace(spaceDesignation))
                throw new ArgumentException("Space designation is required.");

            Id = Guid.NewGuid();
            HubId = hubId;
            SpaceDesignation = spaceDesignation;
        }

        public void Occupy(Guid vehicleId, DateTime fromUtc, DateTime untilUtc)
        {
            if (IsOccupied)
                throw new InvalidOperationException($"Space {SpaceDesignation} is already occupied until {OccupiedUntilUtc}.");
            if (fromUtc >= untilUtc)
                throw new ArgumentException("Occupancy start time must be before end time.");

            OccupiedByVehicleId = vehicleId;
            OccupiedFromUtc = fromUtc;
            OccupiedUntilUtc = untilUtc;
        }

        public void Vacate()
        {
            OccupiedByVehicleId = null;
            OccupiedFromUtc = null;
            OccupiedUntilUtc = null;
        }
    }
}
