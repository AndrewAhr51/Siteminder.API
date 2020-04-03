using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.ResourceParameters;
using System;

namespace Siteminder.API.Models
{
    public interface ICustomerAccountRepository
    {
        void CreateCustomerAccount(CustomerAccount customerAccount);
        void CreateCustomerAccount(Guid customerId, CustomerAccount customerAccount);
        bool CustomerAccountExists(Guid customerId);
        void DeleteCustomerAccount(CustomerAccount customerAccount);
        public PagedList<CustomerAccount> GetCustomerAccounts(CustomerAccountResourceParameters customerResourseParameters);
        CustomerAccount GetCustomerAccount(Guid companyId, Guid customerId);
        void UpdateCustomerAccount(CustomerAccount customerAccount);
        bool Save();
    }
}
