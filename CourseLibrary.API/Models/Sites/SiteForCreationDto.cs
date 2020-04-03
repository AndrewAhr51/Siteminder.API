using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class SiteForCreationDto
    {
        public string CompanyId { get; set; }
        public string SiteTypeId { get; set; }
        public string SiteName { get; set;}
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}
