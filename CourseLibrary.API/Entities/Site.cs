using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Site
    {
        [Key]
        public Guid Id { get; set; }

        [Required (ErrorMessage="Please enter a site name")]
        [MaxLength(50)]
        public string SiteName { get; set; }

        [Required(ErrorMessage = "Please enter a site address")]
        [MaxLength(50)]
        public string Address1 { get; set; }

        [MaxLength(50)]
        public string Address2 { get; set; }

        [Required(ErrorMessage = "Please enter a site city")]
        [MaxLength(50)]
        public string City { get; set; }

        [Required(ErrorMessage = "Please select a site state")]
        [MaxLength(2)]
        public string State { get; set; }

        [Required(ErrorMessage = "Please enter a site zipcode")]
        [MaxLength(10)]
        public string ZipCode { get; set; }
        
        [ForeignKey("CompanyId")]
        public Guid CompanyId { get; set; }

        [ForeignKey("SiteTypeId")]
        public Guid SiteTypeId { get; set; }
        public ICollection<Terminal> Terminals { get; set; }
         = new List<Terminal>();

        public ICollection<Discount> Discounts { get; set; }
        = new List<Discount>();

    }
}
