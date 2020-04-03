using Siteminder.API.DbContexts;
using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.Models;
using Siteminder.API.ResourceParameters;
using System;
using System.Linq;

namespace Siteminder.API.Services
{
    public class UserAccountRepository : IUserAccountRepository, IDisposable
    {
        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public UserAccountRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new Exception(nameof(context));

            _propertyMappingService = propertyMappingService ??
               throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddUserAccount(UserAccount userAccount)
        {
            if (userAccount == null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }

            userAccount.Id = Guid.NewGuid();

            _context.UserAccounts.Add(userAccount);
        }

        public void AddUserAccount(Guid userAccountId, UserAccount userAccount)
        {
            if (userAccount == null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }

            userAccount.Id = userAccountId;

            _context.UserAccounts.Add(userAccount);
        }

        public void DeleteUserAccount(UserAccount userAccount)
        {
            if (userAccount == null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }

            _context.UserAccounts.Remove(userAccount);
        }

        public UserAccount GetUserAccount(Guid companyId, Guid userAccountId)
        {
            if (userAccountId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userAccountId));
            }

            return _context.UserAccounts.Where(a => a.Id == userAccountId || a.CompanyId == companyId).FirstOrDefault();

        }

        public PagedList<UserAccount> GetUserAccounts(UserAccountResourceParameters userAccountResourseParameters)
        {
            if (userAccountResourseParameters == null)
            {
                throw new ArgumentNullException(nameof(userAccountResourseParameters));
            }

            var collection = _context.UserAccounts as IQueryable<UserAccount>;

            collection = collection.Where(a => a.CompanyId == userAccountResourseParameters.CompanyId);

            if (!string.IsNullOrWhiteSpace(userAccountResourseParameters.SearchQuery))
            {
                var searchQuery = userAccountResourseParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.UserName.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(userAccountResourseParameters.OrderBy))
            {
                var userAccountPropertyMappingDictionary = _propertyMappingService.GetUserAccountPropertyMapping<UserAccountDto, UserAccount>();

                collection = collection.ApplySort(userAccountResourseParameters.OrderBy, userAccountPropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<UserAccount>.Create(collection,
                userAccountResourseParameters.PageNumber,
                userAccountResourseParameters.PageSize);
        }

        public void PatchUserAccount(UserAccount userAccount)
        {
            if (userAccount == null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }

            _context.UserAccounts.Update(userAccount);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool UserAccountExists(Guid userAccountId)
        {
            if (userAccountId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userAccountId));
            }

            return _context.UserAccounts.Any(a => a.Id == userAccountId);
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
