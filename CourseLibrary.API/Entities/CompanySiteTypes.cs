using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class CompanySiteTypes
    {
        [Key]
        [ForeignKey("CompanyId")]
        public Guid CompanyId { get; set; }
        [Key]
        [ForeignKey("SiteTypeId")]
        public Guid SiteTypeId { get; set; }
    }
}
