using Siteminder.API.Entities;
using Siteminder.API.ResourceParameters;
using Siteminder.API.Helper;
using System;

namespace Siteminder.API.Services
{
    public interface IUserAccountRepository
    {
        void AddUserAccount(UserAccount userAccount);
        void AddUserAccount(Guid userAccountId, UserAccount userAccount);
        bool UserAccountExists(Guid userAccountId);
        void DeleteUserAccount(UserAccount userAccount);
        public PagedList<UserAccount> GetUserAccounts(UserAccountResourceParameters userAccountResourseParameters);
        UserAccount GetUserAccount(Guid companyId, Guid userAccountId);
        void PatchUserAccount(UserAccount userAccount);
        bool Save();
    }
}
