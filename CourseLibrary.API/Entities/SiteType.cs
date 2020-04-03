using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class SiteType
    {
        [Key]
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public ICollection<Site> Sites { get; set; }
          = new List<Site>();

    }
}
