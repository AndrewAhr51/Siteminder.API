using Siteminder.API.Entities;
using Siteminder.API.ResourceParameters;
using Siteminder.API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public interface ISiteTypeRepository
    {
        void AddSiteType(SiteType siteType);
        void AddSiteType(Guid siteTypeId, SiteType siteType);
        CompanySiteTypes GetCompanySiteTypeAssignment(Guid companyId, Guid siteTypeId);
        void CreateCompanySiteTypeAssignment(CompanySiteTypes companySiteType);
        void DeleteCompanySiteTypeAssignment(CompanySiteTypes companySiteType);
        bool SiteTypeExists(Guid siteTypeId);
        void DeleteSiteType(SiteType siteType);
        public PagedList<SiteType> GetSiteTypes(SiteTypeResourceParameters siteTypeResourseParameters);
        SiteType GetSiteType(Guid siteTypeId);
        void UpdateSiteType(SiteType siteType);
        bool Save();
    }
}
