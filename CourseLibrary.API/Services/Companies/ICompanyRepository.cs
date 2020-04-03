using Siteminder.API.Entities;
using Siteminder.API.ResourceParameters;
using Siteminder.API.Helper;
using System;

namespace Siteminder.API.Services
{
    public interface ICompanyRepository
    {
        void AddCompany(Company company);
        void AddCompany(Guid companyId, Company company);
        bool CompanyExists(Guid companyId);
        void DeleteCompany(Company company);
        public PagedList<Company> GetCompanies(CompanyResourceParameters companyResourseParameters);
        Company GetCompany(Guid companyId);
        void UpdateCompany(Company company);
        bool Save();
    }
}
