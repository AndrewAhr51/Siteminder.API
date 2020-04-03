
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class DispenserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DispenserType { get; set; }
        public string PulserType { get; set; }
        public string DispenserId { get; set; }
        public string HVDisplayNumber { get; set; }
        public string TotalizerReading { get; set; }
        public string MaxTotalizerDigits { get; set; } = "9";
        public string VolumeUnit { get; set; }
        public string TerminalId { get; set; }
        public string FuelId { get; set; }
        public string ScheduleId { get; set; }
    }
}
