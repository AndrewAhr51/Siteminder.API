using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Entities
{
    public class CustomerAccount
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Please select an account type")]
        public string AccountType { get; set; }

        [Required(ErrorMessage = "Please select an payment type")]
        public string PaymentType { get; set; }

        [Required(ErrorMessage = "Please Enter an Credit limit type")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditLimit { get; set; }

        [Required(ErrorMessage = "Please Enter an CustomerName type")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Please Enter an Tail Number")]
        public string TailNumber { get; set; }

        [Required(ErrorMessage = "Please enter an account number")]
        public string AccountNumber { get; set; }

        public string ExternalAccountReference { get; set; }

        public string Identifer { get; set; } //Tail number or Vehicle Id

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }

        [Required(ErrorMessage = "Please select wheter an acount code is required")]
        public bool AccountingCodeRequired { get; set; } = false;

        [Required(ErrorMessage = "Please Select the Card Type")]
        public string CardTypeID { get; set; }

        public string EmbossedNumber { get; set; }

        public string CardHolderName { get; set; }

        public int Status { get; set; } = 1;

        [Required(ErrorMessage = "Please select an account name")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Please enter the Contact Name")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "Please enter the account holder email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter the account holder address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        [Required(ErrorMessage = "Please enter the account holder city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter the account holder state")]
        [MaxLength(2)]
        public string State { get; set; }

        [Required(ErrorMessage = "Please enter the account holder zipcode")]
        [MaxLength(10)]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Please enter the account holder phone number")]
        public string Phone { get; set; }

        public string Fax { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPerGallon { get; set; }

        public int PinCode { get; set; } = 0;

        [ForeignKey("CompanyId")]
        public Guid CompanyId { get; set; }
        
        public ICollection<Transaction> Transactions { get; set; }
           = new List<Transaction>();
        public ICollection<AssignedTerminals> AssignedTerminals { get; set; }
          = new List<AssignedTerminals>();

    }
}
