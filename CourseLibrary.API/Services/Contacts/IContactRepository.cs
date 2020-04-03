using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public interface IContactRepository
    {
        public PagedList<Contact> GetCompanyContacts(Guid companyId, ContactResourceParameters contactResourseParameters);
        public Contact GetCompanyContact(Guid companyId, Guid contactId);
        public SiteContacts GetCompanySiteContact(Guid companyId, Guid siteId,Guid contactId);
        public TerminalContacts GetCompanySiteTerminalContact(Guid companyId, Guid terminalId, Guid contactId);
        public void CreateCompanyContact(Guid companyId, Contact contact);
        public void CreateCompanySiteContact(SiteContacts siteContact);
        public void CreateCompanySiteTerminalContact(TerminalContacts terminalContact);
        public void PatchContact(Contact contact);
        public void DeleteCompanyContact(Contact contact);
        public bool ContactExists(Guid contactId);
        public void CreateCompanyContactsAssignment(CompanyContacts companyContacts);
        public void CreateSiteContactsAssignment(SiteContacts siteContacts);
        public void DeleteSiteContactsAssignment(SiteContacts siteContacts);
        public void CreateTerminalContactsAssignment(TerminalContacts terminalContacts);
        public void DeleteTerminalContactsAssignment(TerminalContacts terminalContacts);
        bool Save();
    }
}
