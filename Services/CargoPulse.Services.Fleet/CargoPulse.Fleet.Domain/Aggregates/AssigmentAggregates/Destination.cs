using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.VehicleAssigmentAggregates
{
    public record Destination(double Latitude, double Longitude, Guid? HubId = null);
}
