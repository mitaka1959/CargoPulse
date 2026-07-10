using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.VehicleAggregates
{
    public enum VehicleStatus
    {
        Available = 1,
        Maintenance = 2,
        OnAssignment = 3,
        Reserved = 4,
        Retired = 5
    }
}
