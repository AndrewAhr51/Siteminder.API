using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siteminder.API.DbContexts;
using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.Models;
using Siteminder.API.ResourceParameters;

namespace Siteminder.API.Services
{
    public class CompanyRepository : ICompanyRepository, IDisposable
    {
        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public CompanyRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new Exception(nameof(context));

            _propertyMappingService = propertyMappingService ??
               throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            company.Id = Guid.NewGuid();

            _context.Company.Add(company);

        }

        public void AddCompany(Guid companyId, Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            company.Id = companyId;

            _context.Company.Add(company);

        }

        public bool CompanyExists(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return _context.Company.Any(a => a.Id == companyId);

        }

        public void DeleteCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            _context.Company.Remove(company);
        }

        public PagedList<Company> GetCompanies(CompanyResourceParameters companiesResourceParameters)
        {
            if (companiesResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(companiesResourceParameters));
            }

            var collection = _context.Company as IQueryable<Company>;

            if (!string.IsNullOrWhiteSpace(companiesResourceParameters.Type))
            {
                var type = companiesResourceParameters.Type.Trim();
                collection = collection.Where(a => a.Type == type);
            }

            if (!string.IsNullOrWhiteSpace(companiesResourceParameters.SearchQuery))
            {
                var searchQuery = companiesResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Type.Contains(searchQuery)
                 || a.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(companiesResourceParameters.OrderBy))
            {
                var companyPropertyMappingDictionary = _propertyMappingService.GetCompanyPropertyMapping<CompanyDto, Company>();

                collection = collection.ApplySort(companiesResourceParameters.OrderBy, companyPropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<Company>.Create(collection,
                companiesResourceParameters.PageNumber,
                companiesResourceParameters.PageSize);
        }

        public Company GetCompany(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            return _context.Company.FirstOrDefault(a => a.Id == companyId);
        }
        
        public void UpdateCompany(Company company)
        {
            if (company == null)
            {
                throw new ArgumentNullException(nameof(company));
            }

            _context.Company.Update(company);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
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
