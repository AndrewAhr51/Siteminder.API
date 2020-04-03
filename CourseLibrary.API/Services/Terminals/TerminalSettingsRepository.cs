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
    public class TerminalSettingsRepository : ITerminalSettingsRepository, IDisposable
    {
        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public TerminalSettingsRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
                    }
        public void AddTerminalSettings(TerminalSettings terminalSettings)
        {
            if (terminalSettings == null)
            {
                throw new ArgumentNullException(nameof(terminalSettings));
            }

            terminalSettings.Id = Guid.NewGuid();
            

            _context.TerminalSettings.Add(terminalSettings);
        }

        public void AddTerminalSettings(Guid terminalSettingsId, TerminalSettings terminalSettings)
        {
            if (terminalSettingsId == null)
            {
                throw new ArgumentNullException(nameof(terminalSettingsId));
            }

            terminalSettings.Id = terminalSettingsId;
            
            _context.TerminalSettings.Add(terminalSettings);
        }

        public void DeleteTerminalSettings(TerminalSettings terminalSettings)
        {
            if (terminalSettings == null)
            {
                throw new ArgumentNullException(nameof(terminalSettings));
            }

            _context.TerminalSettings.Remove(terminalSettings);
        }

        public TerminalSettings GetTerminalSettings(Guid terminalId)
        {
            if (terminalId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(terminalId));
            }

            return _context.TerminalSettings.Where(a => a.TerminalId == terminalId).FirstOrDefault();
        }

        public PagedList<TerminalSettings> GetTerminalSettings(TerminalSettingsParameters terminalSettingsParameters)
        {
            if (terminalSettingsParameters == null)
            {
                throw new ArgumentNullException(nameof(terminalSettingsParameters));
            }

            if (terminalSettingsParameters.TerminalId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(terminalSettingsParameters));
            }

            var collection = _context.TerminalSettings as IQueryable<TerminalSettings>;

            collection = collection.Where(a => a.TerminalId == terminalSettingsParameters.TerminalId);
            if (!string.IsNullOrWhiteSpace(terminalSettingsParameters.OrderBy))
            {
                var terminalSettingsPropertyMappingDictionary = _propertyMappingService.GetTerminalSettingsPropertyMapping<TerminalSettingsDto, TerminalSettings>();

                collection = collection.ApplySort(terminalSettingsParameters.OrderBy, terminalSettingsPropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<TerminalSettings>.Create(collection,
                terminalSettingsParameters.PageNumber,
                terminalSettingsParameters.PageSize);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool TerminalSettingExists(Guid terminalSettingsId)
        {
            if (terminalSettingsId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(terminalSettingsId));
            }

            return _context.TerminalSettings.Any(a => a.Id == terminalSettingsId);
        }

        public void UpdateTerminalSettings(Guid terminalSettingsId, TerminalSettings terminalSettings)
        {
            if (terminalSettingsId == null || terminalSettings == null)
            {
                throw new ArgumentNullException(nameof(UpdateTerminalSettings));
            }

            terminalSettings.Id = terminalSettingsId;

            _context.TerminalSettings.Update(terminalSettings);
        }

        public bool TerminalHasFillupOption(Guid terminalId)
        {
            var disableFillup = false;
            var terminalSetting = _context.TerminalSettings
                        .Where(a => a.TerminalId == terminalId).FirstOrDefault();

            if (terminalSetting.DisableFillupKey == true)
            {
                disableFillup = true;
            }

            return disableFillup;
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