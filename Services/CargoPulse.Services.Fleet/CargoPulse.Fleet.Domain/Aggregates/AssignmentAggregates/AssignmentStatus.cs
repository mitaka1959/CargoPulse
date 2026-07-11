using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoPulse.Fleet.Domain.Aggregates.AssigmentAggregates
{
    public enum AssignmentStatus
    {
        Scheduled = 1,
        Cancelled = 2,
        Ongoing = 3,
        Completed = 4,
    }
}
