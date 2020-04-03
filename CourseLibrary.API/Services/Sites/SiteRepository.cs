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
    public class SiteRepository : ISiteRepository, IDisposable
    {
        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public SiteRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddSite(Site site)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }
            
            site.Id = Guid.NewGuid();

            _context.Site.Add(site);
        }

        public void AddSite(Guid siteId, Site site)
        {

            if (siteId == null)
            {
                throw new ArgumentNullException(nameof(siteId));
            }

            site.Id = siteId;

            _context.Site.Add(site);

        }
        public void DeleteSite(Site site)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            _context.Site.Remove(site);
        }
        public Site GetSite(Guid companyId, Guid siteTypeId, Guid siteId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            if (siteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(siteId));
            }

            return _context.Site.Where(a => a.CompanyId == companyId || a.SiteTypeId == siteTypeId || a.Id == siteId).FirstOrDefault();

        }

        public PagedList<Site> GetSites(SiteResourceParameters siteResourseParameters)
        {
            if (siteResourseParameters == null)
            {
                throw new ArgumentNullException(nameof(siteResourseParameters));
            }

            if (siteResourseParameters.CompanyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(siteResourseParameters));
            }

            var collection = _context.Site as IQueryable<Site>;

            if (siteResourseParameters.SiteTypeId !=  Guid.Empty)
            {
                collection = collection.Where(a => a.CompanyId == siteResourseParameters.CompanyId || a.SiteTypeId == siteResourseParameters.SiteTypeId);
            }
            else
            {
                collection = collection.Where(a => a.CompanyId == siteResourseParameters.CompanyId);
            }

            if (!string.IsNullOrWhiteSpace(siteResourseParameters.SearchQuery))
            {
                var searchQuery = siteResourseParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.SiteName.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(siteResourseParameters.OrderBy))
            {
                var sitePropertyMappingDictionary = _propertyMappingService.GetSitePropertyMapping<SiteDto, Site>();

                collection = collection.ApplySort(siteResourseParameters.OrderBy, sitePropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<Site>.Create(collection,
                siteResourseParameters.PageNumber,
                siteResourseParameters.PageSize);
        }
        public PagedList<Site> GetSites(Guid companyId, SiteResourceParameters siteResourseParameters)
        {
            if (siteResourseParameters == null)
            {
                throw new ArgumentNullException(nameof(siteResourseParameters));
            }

            var collection = _context.Site as IQueryable<Site>;

            if (siteResourseParameters.CompanyId != Guid.Empty)
            {
                collection = collection.Where(a => a.CompanyId == siteResourseParameters.CompanyId);
            }

            if (!string.IsNullOrWhiteSpace(siteResourseParameters.SearchQuery))
            {
                var searchQuery = siteResourseParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.SiteName.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(siteResourseParameters.OrderBy))
            {
                var sitePropertyMappingDictionary = _propertyMappingService.GetSitePropertyMapping<SiteDto, Site>();

                collection = collection.ApplySort(siteResourseParameters.OrderBy, sitePropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<Site>.Create(collection,
                siteResourseParameters.PageNumber,
                siteResourseParameters.PageSize);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool SiteExists(Guid siteId)
        {
            if (siteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(siteId));
            }

            return _context.Site.Any(a => a.Id == siteId);
        }

        public void UpdateSite(Site site)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            _context.Site.Update(site);
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
