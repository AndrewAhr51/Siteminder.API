using Siteminder.API.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    [PhoneMustBeDifferentFromFax(ErrorMessage = "The phone number must be different than the fax.")]
    public abstract class ContactForManipulationDto
    {
        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Phone { get; set; }
        
        public virtual string Fax { get; set; }

        public virtual string Email { get; set; }

        public virtual string CompanyId { get; set; }

        public virtual string SiteTypeId { get; set; }
    }
}
