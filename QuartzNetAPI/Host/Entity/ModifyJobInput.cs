using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Entity
{
    public class ModifyJobInput
    {
        public ScheduleEntity NewScheduleEntity { get; set; }
        public ScheduleEntity OldScheduleEntity { get; set; }
    }
}
