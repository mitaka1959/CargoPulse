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
        Ongoing = 2,
        Ended = 3,
    }
}
