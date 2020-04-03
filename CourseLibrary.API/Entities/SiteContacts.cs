using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class SiteContacts
    {
        [Key]
        public Guid SiteId { get; set; }
        [Key]
        public Guid ContactId { get; set; }
    }
}
