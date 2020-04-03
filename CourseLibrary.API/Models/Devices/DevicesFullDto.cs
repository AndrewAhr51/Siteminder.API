using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class DevicesFullDto
    {
        public string Id { get; set; }
        public string DeviceName { get; set; }
        public string ModelName { get; set; }
        public string SerialNumber { get; set; }
        public string Description { get; set; }

    }
}
