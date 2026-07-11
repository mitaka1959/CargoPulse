using CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates;
using CargoPulse.Fleet.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CargoPulse.Fleet.Domain.Aggregates.DriverAggregates
{
    public enum DriverStatus
    {
        Available = 1,
        OnAssignment,
        OnVacation,
        OffDuty,
        Suspended,
        Retired
    }

    public class Driver : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string LicenseNumber { get; private set; } = string.Empty;
        public DriverStatus Status { get; private set; } = DriverStatus.Available;
        public GeoLocation HomeBaseLocation { get; private set; } = null!;

        private Driver() { }

        public Driver(Guid id, string name, string licenseNumber, GeoLocation homeBaseLocation)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Driver name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(licenseNumber))
                throw new ArgumentException("License number cannot be empty.", nameof(licenseNumber));

            Id = id;
            Name = name;
            LicenseNumber = licenseNumber;
            Status = DriverStatus.Available;
            HomeBaseLocation = homeBaseLocation ?? throw new ArgumentNullException(nameof(homeBaseLocation));
        }

        // State Transition Domain Behaviors (previously the DriverStatus enum had no behaviour at all).
        public void AssignToRoute()
        {
            if (Status != DriverStatus.Available)
                throw new InvalidOperationException($"Driver must be Available to be assigned (current: {Status}).");

            Status = DriverStatus.OnAssignment;
        }

        public void CompleteRoute()
        {
            if (Status != DriverStatus.OnAssignment)
                throw new InvalidOperationException("Driver is not currently on an assignment.");

            Status = DriverStatus.Available;
        }

        public void GoOffDuty()
        {
            if (Status != DriverStatus.Available)
                throw new InvalidOperationException("Only an available driver can go off duty.");

            Status = DriverStatus.OffDuty;
        }

        public void StartVacation()
        {
            if (Status is not (DriverStatus.Available or DriverStatus.OffDuty))
                throw new InvalidOperationException($"A driver in status '{Status}' cannot start vacation.");

            Status = DriverStatus.OnVacation;
        }

        public void ReturnToDuty()
        {
            if (Status is not (DriverStatus.OffDuty or DriverStatus.OnVacation))
                throw new InvalidOperationException($"A driver in status '{Status}' cannot return to duty.");

            Status = DriverStatus.Available;
        }

        public void Suspend()
        {
            if (Status == DriverStatus.Retired)
                throw new InvalidOperationException("A retired driver cannot be suspended.");

            Status = DriverStatus.Suspended;
        }

        public void Reinstate()
        {
            if (Status != DriverStatus.Suspended)
                throw new InvalidOperationException("Only a suspended driver can be reinstated.");

            Status = DriverStatus.Available;
        }

        public void Retire()
        {
            if (Status == DriverStatus.OnAssignment)
                throw new InvalidOperationException("Cannot retire a driver while they are on assignment.");

            Status = DriverStatus.Retired;
            Delete();
        }
    }
}