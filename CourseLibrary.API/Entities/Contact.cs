using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class Contact
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage ="Please fill out the first name.")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please fill out the last name.")]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please fill out the persons title.")]
        [MaxLength(30)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter a country code.")]
        [MaxLength(2)]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Please enter the person's phone number.")]
        [MaxLength(12)]
        public string Phone { get; set; }

        [MaxLength(12)]
        public string Mobile { get; set; }

        [MaxLength(12)]
        public string Fax { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [ForeignKey("CompanyId")]
        public Guid CompanyId { get; set; }

    }
}
