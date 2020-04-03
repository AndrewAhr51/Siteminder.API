using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Models.CustomerAccounts
{
    public class CustomerAccountFullDto
    {
        public string Id { get; set; }

        public string AccountType { get; set; }

        public string PaymentType { get; set; }

        public string CreditLimit { get; set; }

        public string CustomerName { get; set; }

        public string TailNumber { get; set; }

        public string AccountNumber { get; set; }

        public string ExternalAccountReference { get; set; }

        public string Identifer { get; set; }

        public string Balance { get; set; }

        public string AccountingCodeRequired { get; set; }

        public string CardTypeID { get; set; }

        public string EmbossedNumber { get; set; }

        public string CardHolderName { get; set; }

        public string Status { get; set; } 

        public string AccountName { get; set; }

        public string ContactName { get; set; }

        public string EmailAddress { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string DiscountPerGallon { get; set; }

        public string PinCode { get; set; }
        public string CompanyId { get; set; }
    }
}
