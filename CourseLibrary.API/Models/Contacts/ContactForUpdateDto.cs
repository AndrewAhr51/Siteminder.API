using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models
{
    public class ContactForUpdateDto 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string CountryCode { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
    }
}
