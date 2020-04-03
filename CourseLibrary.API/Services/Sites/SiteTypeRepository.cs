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
    public class SiteTypeRepository : ISiteTypeRepository, IDisposable
    {
        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public SiteTypeRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new Exception(nameof(context));

            _propertyMappingService = propertyMappingService ??
               throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddSiteType(SiteType siteType)
        {
            if (siteType == null)
            {
                throw new ArgumentNullException(nameof(siteType));
            }

            siteType.Id = Guid.NewGuid();

            _context.SiteType.Add(siteType);

        }

        public void AddSiteType(Guid siteTypeId, SiteType siteType)
        {
            if (siteType == null)
            {
                throw new ArgumentNullException(nameof(siteType));
            }

            siteType.Id = siteTypeId;

            _context.SiteType.Add(siteType);

        }

        public CompanySiteTypes GetCompanySiteTypeAssignment(Guid companyId, Guid siteTypeId)
        {
            if (siteTypeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(siteTypeId));
            }

            return _context.CompanySiteTypes.FirstOrDefault(a => a.CompanyId == companyId || a.SiteTypeId == siteTypeId);
        }
        public void CreateCompanySiteTypeAssignment(CompanySiteTypes companySiteTypes)
        {
            if (companySiteTypes == null)
            {
                throw new ArgumentNullException(nameof(companySiteTypes));
            }
            
            _context.CompanySiteTypes.Add(companySiteTypes);
        }

        public void DeleteCompanySiteTypeAssignment(CompanySiteTypes companySiteTypes)
        {
            if (companySiteTypes == null)
            {
                throw new ArgumentNullException(nameof(companySiteTypes));
            }

            _context.CompanySiteTypes.Remove(companySiteTypes);
        }

        public bool SiteTypeExists(Guid siteTypeId)
        {
            if (siteTypeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(siteTypeId));
            }

            return _context.SiteType.Any(a => a.Id == siteTypeId);

        }

        public void DeleteSiteType(SiteType siteType)
        {
            if (siteType == null)
            {
                throw new ArgumentNullException(nameof(siteType));
            }

            _context.SiteType.Remove(siteType);
        }

        public PagedList<SiteType> GetSiteTypes(SiteTypeResourceParameters siteTypeResourceParameters)
        {
            if (siteTypeResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(siteTypeResourceParameters));
            }

            var collection = _context.SiteType as IQueryable<SiteType>;

            if (!string.IsNullOrWhiteSpace(siteTypeResourceParameters.SearchQuery))
            {
                var searchQuery = siteTypeResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Type.Contains(searchQuery)
                 || a.Type.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(siteTypeResourceParameters.OrderBy))
            {
                var siteTypePropertyMappingDictionary = _propertyMappingService.GetSiteTypePropertyMapping<SiteTypeDto, SiteType>();

                collection = collection.ApplySort(siteTypeResourceParameters.OrderBy, siteTypePropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<SiteType>.Create(collection,
                siteTypeResourceParameters.PageNumber,
                siteTypeResourceParameters.PageSize);
        }

        public SiteType GetSiteType(Guid siteTypeId)
        {
            if (siteTypeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(siteTypeId));
            }

            return _context.SiteType.FirstOrDefault(a => a.Id == siteTypeId);
        }
        
        public void UpdateSiteType(SiteType siteType)
        {
            if (siteType == null)
            {
                throw new ArgumentNullException(nameof(siteType));
            }

            _context.SiteType.Update(siteType);
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
