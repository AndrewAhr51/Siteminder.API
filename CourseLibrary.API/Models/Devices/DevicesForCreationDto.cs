using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class DevicesForCreationDto
    {
        public string DeviceName { get; set; }
        public string ModelName { get; set; }
        public string SerialNumber { get; set; }
        public string Description { get; set; }

        public Guid TerminalId { get; set; }

    }
}
