using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Company
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please enter a company name")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a company type")]
        [MaxLength(30)]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please enter a company address")]
        [MaxLength(50)]
        public string Address1 { get; set; }

        [MaxLength(50)]
        public string Address2 { get; set; }

        [Required(ErrorMessage = "Please enter a company city")]
        [MaxLength(50)]
        public string City { get; set; }

        [Required(ErrorMessage = "Please select a state")]
        [MaxLength(2)]
        public string State { get; set; }

        [Required(ErrorMessage = "Please enter a zipcode")]
        [MaxLength(10)]
        public string ZipCode { get; set; }
        public ICollection<Contact> Contacts { get; set; }
          = new List<Contact>();
        public ICollection<Site> Sites { get; set; }
          = new List<Site>();
        public ICollection<CustomerAccount> Accounts { get; set; }
          = new List<CustomerAccount>();
        public ICollection<AuthorizedCard> AuthorizedCards { get; set; }
          = new List<AuthorizedCard>();
        public ICollection<CompanySiteTypes> CompanySiteTypes { get; set; }
         = new List<CompanySiteTypes>();



    }
}
