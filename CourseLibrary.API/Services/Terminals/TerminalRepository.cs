using Siteminder.API.DbContexts;
using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.Models;
using Siteminder.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public class TerminalRepository : ITerminalRepository, IDisposable
    {
        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public TerminalRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddTerminal(Terminal terminal)
        {
            if (terminal == null)
            {
                throw new ArgumentNullException(nameof(terminal));
            }

            _context.Terminal.Add(terminal);
        }
        public void AddTerminal(Guid siteId, Guid terminalId, Terminal terminal)
        {
            if (siteId == null || terminalId == null || terminal == null)
            {
                throw new ArgumentNullException(nameof(AddTerminal));
            }

            terminal.Id = terminalId;
            terminal.SiteId = siteId;

            _context.Terminal.Add(terminal);
        }

        public void DeleteTerminal(Terminal terminal)
        {
            if (terminal == null)
            {
                throw new ArgumentNullException(nameof(terminal));
            }

            _context.Terminal.Remove(terminal);
        }

        public Terminal GetTerminal(Guid siteId, Guid terminalId)
        {
            if (siteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(siteId));
            }

            return _context.Terminal.Where(a => a.SiteId == siteId || a.Id == terminalId).FirstOrDefault();

        }

        public PagedList<Terminal> GetTerminals(TerminalResourceParameters terminalResourseParameters)
        {
            if (terminalResourseParameters == null)
            {
                throw new ArgumentNullException(nameof(terminalResourseParameters));
            }

            if (terminalResourseParameters.SiteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(terminalResourseParameters));
            }

            var collection = _context.Terminal as IQueryable<Terminal>;

            collection = collection.Where(a => a.SiteId == terminalResourseParameters.SiteId);

            if (!string.IsNullOrWhiteSpace(terminalResourseParameters.SearchQuery))
            {
                var searchQuery = terminalResourseParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.TerminalName.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(terminalResourseParameters.OrderBy))
            {
                var terminalPropertyMappingDictionary = _propertyMappingService.GetTerminalPropertyMapping<TerminalDto, Terminal>();

                collection = collection.ApplySort(terminalResourseParameters.OrderBy, terminalPropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<Terminal>.Create(collection,
                terminalResourseParameters.PageNumber,
                terminalResourseParameters.PageSize);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool TerminalExists(Guid terminalId)
        {
            if (terminalId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(terminalId));
            }

            return _context.Terminal.Any(a => a.Id == terminalId);
        }

        public void UpdateTerminal(Guid terminalId, Terminal terminal)
        {
            if (terminalId == null || terminal == null)
            {
                throw new ArgumentNullException(nameof(terminalId));
            }

            terminal.Id = terminalId;

            _context.Terminal.Update(terminal);
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
