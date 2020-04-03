using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class SiteContactForCreationDto
    {
        public Guid SiteId { get; set; }
        public Guid ContactId { get; set; }
    }
}
