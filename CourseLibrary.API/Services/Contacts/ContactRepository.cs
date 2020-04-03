using System;
using System.Collections.Generic;
using System.Linq;
using Siteminder.API.DbContexts;
using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.Models;
using Siteminder.API.ResourceParameters;

namespace Siteminder.API.Services
{
    public class ContactRepository : IContactRepository, IDisposable
    {

        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public ContactRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new Exception(nameof(context));

            _propertyMappingService = propertyMappingService ??
               throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void CreateCompanyContact(Guid companyId, Contact Contact)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            if (Contact == null)
            {
                throw new ArgumentNullException(nameof(Contact));
            }

            //Contact.Id = Guid.NewGuid();

            Contact.CompanyId = companyId;
            _context.Contact.Add(Contact);

            //Add to to CompanyContacts assignments
            CompanyContacts companyContacts = new CompanyContacts();
            companyContacts.CompanyId = Contact.CompanyId;
            companyContacts.ContactId = Contact.Id;
            CreateCompanyContactsAssignment(companyContacts);
        }

        public void CreateCompanySiteContact(SiteContacts SiteContacts)
        {
            if (SiteContacts == null)
            {
                throw new ArgumentNullException(nameof(SiteContacts));
            }

            _context.SiteContacts.Add(SiteContacts);

        }

        public void CreateCompanySiteTerminalContact(TerminalContacts terminalContacts)
        {
            if (terminalContacts == null)
            {
                throw new ArgumentNullException(nameof(SiteContacts));
            }
            
            _context.TerminalContacts.Add(terminalContacts);

        }

        public bool ContactExists(Guid contactId)
        {
            if (contactId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(contactId));
            }

            return _context.Contact.Any(a => a.Id == contactId);
        }

        public void DeleteCompanyContact(Contact contact)
        {

            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            _context.Contact.Remove(contact);

            //remove the company assignment
            CompanyContacts companyContacts = new CompanyContacts();
            companyContacts.ContactId = contact.Id;
            DeleteCompanyContactsAssignment(companyContacts);

            //remove the site assignment
            SiteContacts siteContacts = new SiteContacts();
            siteContacts.ContactId = contact.Id;
            DeleteSiteContactsAssignment(siteContacts);

            //remove the terminal assignment
            TerminalContacts terminalContacts = new TerminalContacts();
            terminalContacts.ContactId = contact.Id;
            DeleteTerminalContactsAssignment(terminalContacts);
        }

        public PagedList<Contact> GetCompanyContacts(Guid companyId, ContactResourceParameters contactResourseParameters)
        {
            if (contactResourseParameters == null)
            {
                throw new ArgumentNullException(nameof(contactResourseParameters));
            }

            var collection = _context.Contact as IQueryable<Contact>;

            if (!string.IsNullOrWhiteSpace(contactResourseParameters.SearchQuery))
            {
                var searchQuery = contactResourseParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.FirstName.Contains(searchQuery) || a.LastName.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(contactResourseParameters.OrderBy))
            {
                var contactPropertyMappingDictionary = _propertyMappingService.GetContactPropertyMapping<ContactDto, Contact>();

                collection = collection.ApplySort(contactResourseParameters.OrderBy, contactPropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<Contact>.Create(collection,
                contactResourseParameters.PageNumber,
                contactResourseParameters.PageSize);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
        public void PatchContact(Contact contact)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            _context.Contact.Update(contact);
        }

        public void DeleteCompanyContactsAssignment(CompanyContacts companyContacts)
        {
            if (companyContacts == null)
            {
                throw new ArgumentNullException(nameof(companyContacts));
            }

            _context.CompanyContacts.Remove(companyContacts);
        }

        public void CreateSiteContactsAssignment(SiteContacts siteContacts)
        {
            if (siteContacts == null)
            {
                throw new ArgumentNullException(nameof(siteContacts));
            }

            _context.SiteContacts.Add(siteContacts);
        }

        public void DeleteSiteContactsAssignment(SiteContacts siteContacts)
        {
            if (siteContacts == null)
            {
                throw new ArgumentNullException(nameof(siteContacts));
            }

            _context.SiteContacts.Remove(siteContacts);
        }

        public void CreateCompanyContactsAssignment(CompanyContacts companyContacts)
        {
            if (companyContacts == null)
            {
                throw new ArgumentNullException(nameof(companyContacts));
            }

            _context.CompanyContacts.Add(companyContacts);
        }

        public void CreateTerminalContactsAssignment(TerminalContacts terminalContacts)
        {
            if (terminalContacts == null)
            {
                throw new ArgumentNullException(nameof(terminalContacts));
            }

            _context.TerminalContacts.Add(terminalContacts);
        }

        public void DeleteTerminalContactsAssignment(TerminalContacts terminalContacts)
        {
            if (terminalContacts == null)
            {
                throw new ArgumentNullException(nameof(terminalContacts));
            }

            _context.TerminalContacts.Remove(terminalContacts);
        }

        public Contact GetCompanyContact(Guid companyId, Guid contactId)
        {
            Contact contact = new Contact();

            if (companyId == null || contactId == null)
            {
                throw new ArgumentNullException(nameof(GetCompanyContact));
            }

            var companyContact = (from c in _context.Contact
                                  join cc in _context.CompanyContacts on c.Id equals cc.ContactId
                                  join co in _context.Company on cc.CompanyId equals co.Id
                                  where c.Id == contactId || co.Id == companyId
                                  select new { c.Id, c.FirstName, c.LastName, c.Title, c.CountryCode, c.Phone, c.Mobile, c.Fax, c.Email });

            foreach (var item in companyContact)
            {
                contact.Id = item.Id;
                contact.FirstName = item.FirstName;
                contact.LastName = item.LastName;
                contact.Title = item.Title;
                contact.CountryCode = item.CountryCode;
                contact.Phone = item.Phone;
                contact.Mobile = item.Mobile;
                contact.Fax = item.Fax;
                contact.Email = item.Email;
            }

            return contact;
        }

        public SiteContacts GetCompanySiteContact(Guid companyId,Guid siteId, Guid contactId)
        {
            SiteContacts contact = new SiteContacts();

            if (siteId == null || contactId == null)
            {
                throw new ArgumentNullException(nameof(GetCompanySiteContact));
            }

            var siteContact = (from c in _context.Company
                               join con in _context.Contact on c.Id equals con.CompanyId
                               join sc in _context.SiteContacts on con.Id equals sc.ContactId
                               join s in _context.Site on sc.SiteId equals s.Id
                               where con.Id == contactId || sc.SiteId == siteId
                               select new { s.Id, sc.ContactId, s.SiteName, con.FirstName, con.LastName, con.Title, con.CountryCode, con.Phone, con.Mobile, con.Fax, con.Email });

            foreach (var item in siteContact)
            {
                contact.SiteId = item.Id;
                contact.ContactId = item.ContactId;
                /* contact.FirstName = item.FirstName;
                contact.LastName = item.LastName;
                contact.Title = item.Title;
                contact.CountryCode = item.CountryCode;
                contact.Phone = item.Phone;
                contact.Mobile = item.Mobile;
                contact.Fax = item.Fax;
                contact.Email = item.Email; */
            }
            return contact;
        }

        public TerminalContacts GetCompanySiteTerminalContact(Guid companyId, Guid terminalId, Guid contactId)
        {
            TerminalContacts contact = new TerminalContacts();

            if (terminalId == null || contactId == null)
            {
                throw new ArgumentNullException(nameof(GetCompanySiteTerminalContact));
            }

            var terminalContact = (from c in _context.Company
                                   join con in _context.Contact on c.Id equals con.CompanyId
                                   join tc in _context.TerminalContacts on con.Id equals tc.ContactId
                                   join t in _context.Terminal on tc.TerminalId equals t.Id
                                   where con.Id == contactId || tc.TerminalId == terminalId
                                   select new {tc.ContactId, tc.TerminalId, t.TerminalName, con.Id, con.FirstName, con.LastName, con.Title, con.CountryCode, con.Phone, con.Mobile, con.Fax, con.Email });

            foreach (var item in terminalContact)
            {
                contact.ContactId = item.ContactId;
                contact.TerminalId = item.TerminalId;
                //contact.Id = item.Id;
                //contact.FirstName = item.FirstName;
                //contact.LastName = item.LastName;
                //contact.Title = item.Title;
                //contact.CountryCode = item.CountryCode;
                //contact.Phone = item.Phone;
                //contact.Mobile = item.Mobile;
                //contact.Fax = item.Fax;
                //contact.Email = item.Email;
            }
            return contact;
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
