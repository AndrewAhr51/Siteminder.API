using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models.Tanks
{
    public class TankForCreationDto
    {

        public string Name { get; set; }

        public int Description { get; set; }

        public int Size { get; set; }

        public int Level { get; set; }

        public int AlarmPoint { get; set; }

        public Guid TerminalId { get; set; }

        public Guid DispenserId { get; set; }

        public Guid FuelTypeId { get; set; }
    }
}
