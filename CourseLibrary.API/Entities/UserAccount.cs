using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siteminder.API.Entities
{
    public class UserAccount
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Please enter a userName")]
        [MaxLength(20)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a First Name")]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a Last Name")]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter a VALID email address"),RegularExpression("^(.+)@(.+)$")]
        [MaxLength(30)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [MaxLength(30)]
        public string Password { get; set; }
        
        [ForeignKey("CompanyId")]
        public Guid CompanyId { get; set; }

        [ForeignKey("RoleId")]
        public Guid RoleId { get; set; }
    }
}
