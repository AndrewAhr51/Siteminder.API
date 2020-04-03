using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public interface ISiteRepository
    {
        void AddSite(Site site);
        void AddSite(Guid SiteId, Site site);
        void DeleteSite(Site site);
        Site GetSite(Guid companyId, Guid siteTypeId,  Guid siteId);
        public PagedList<Site> GetSites(SiteResourceParameters siteResourseParameters);
        void UpdateSite(Site site);
        bool SiteExists(Guid siteId);
        bool Save();
    }
}
