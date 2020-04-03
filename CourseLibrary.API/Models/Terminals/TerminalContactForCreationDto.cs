using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class TerminalContactForCreationDto
    {
        public Guid TerminalId { get; set; }
        public Guid ContactId { get; set; }
    }
}

