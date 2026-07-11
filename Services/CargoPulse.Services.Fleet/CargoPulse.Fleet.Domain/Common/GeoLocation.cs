using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Common
{
    public record GeoLocation(double Latitude, double Longitude, Guid? HubId = null);
}
