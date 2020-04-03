using System;
using System.Linq;
using Siteminder.API.DbContexts;
using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.Models;
using Siteminder.API.Models.CustomerAccounts;
using Siteminder.API.ResourceParameters;


namespace Siteminder.API.Services.CustomerAccounts
{
    public class CustomerAccountRepository : ICustomerAccountRepository, IDisposable
    {
        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public CustomerAccountRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new Exception(nameof(context));

            _propertyMappingService = propertyMappingService ??
               throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void CreateCustomerAccount(Entities.CustomerAccount customerAccount)
        {

            if (customerAccount == null)
            {
                throw new ArgumentNullException(nameof(Contact));
            }

            _context.CustomerAccounts.Add(customerAccount);
        }

        public void CreateCustomerAccount(Guid customerAccountId, Entities.CustomerAccount customerAccount)
        {
            if (customerAccount == null)
            {
                throw new ArgumentNullException(nameof(Contact));
            }

            customerAccount.Id = customerAccountId;
            _context.CustomerAccounts.Add(customerAccount);
        }

        public bool CustomerAccountExists(Guid customerAccountId)
        {
            if (customerAccountId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(customerAccountId));
            }

            return _context.CustomerAccounts.Any(a => a.Id == customerAccountId);
        }

        public void DeleteCustomerAccount(Entities.CustomerAccount customerAccount)
        {
            if (customerAccount == null)
            {
                throw new ArgumentNullException(nameof(customerAccount));
            }

            _context.CustomerAccounts.Remove(customerAccount);
        }

        public PagedList<CustomerAccount> GetCustomerAccounts(CustomerAccountResourceParameters customerResourseParameters)
        {
            if (customerResourseParameters == null)
            {
                throw new ArgumentNullException(nameof(customerResourseParameters));
            }

            var collection = _context.CustomerAccounts as IQueryable<CustomerAccount>;

            if (!string.IsNullOrWhiteSpace(customerResourseParameters.SearchQuery))
            {
                var searchQuery = customerResourseParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.ContactName.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(customerResourseParameters.OrderBy))
            {
                var customerAccountPropertyMappingDictionary = _propertyMappingService.GetCustomerAccountPropertyMapping<CustomerAccountDto, CustomerAccount>();

                collection = collection.ApplySort(customerResourseParameters.OrderBy, customerAccountPropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<CustomerAccount>.Create(collection,
                customerResourseParameters.PageNumber,
                customerResourseParameters.PageSize);
            
        }

        public CustomerAccount GetCustomerAccount(Guid companyId, Guid customerAccountId)
        {
            if (customerAccountId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(customerAccountId));
            }


            return _context.CustomerAccounts.Where(a => a.Id == customerAccountId ||a.CompanyId == companyId).FirstOrDefault();

        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateCustomerAccount(Entities.CustomerAccount customerAccount)
        {
            if (customerAccount == null)
            {
                throw new ArgumentNullException(nameof(customerAccount));
            }

            _context.CustomerAccounts.Update(customerAccount);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
