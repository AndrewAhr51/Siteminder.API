using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class DispenserFuelDto
    {
        public Guid DispenserId { get; set; }
        public Guid FuelId { get; set; }
    }
}
