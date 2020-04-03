using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class ScheduleDataDto
    {
        public Guid ScheduleDataId { get; set; }

        public string DayOfWeek { get; set; }

        public DateTime OpenTime { get; set; }

        public DateTime CloseTime { get; set; }

    }
}
