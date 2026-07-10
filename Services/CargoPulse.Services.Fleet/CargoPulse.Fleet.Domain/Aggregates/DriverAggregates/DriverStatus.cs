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
        OnAssignment = 2,
        OffDuty = 3,     
        OnVacation = 4,
        Suspended = 5,    
        Retired = 6       
    }
}
