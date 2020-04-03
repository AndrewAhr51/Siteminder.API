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

    public class DispenserRepository : IDispenserRepository, IDisposable
    {

        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public DispenserRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new Exception(nameof(context));

            _propertyMappingService = propertyMappingService ??
               throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddDispenser(Dispenser dispenser)
        {
            if (dispenser == null)
            {
                throw new ArgumentNullException(nameof(dispenser));
            }

            dispenser.Id = Guid.NewGuid();

            _context.Dispensers.Add(dispenser);
        }

        public void AddDispenser(Guid dispenserId, Dispenser dispenser)
        {
            if (dispenser == null)
            {
                throw new ArgumentNullException(nameof(dispenser));
            }

            dispenser.Id = dispenserId;

            _context.Dispensers.Add(dispenser);
        }

        public void DeleteDispenser(Dispenser dispenser)
        {
            if (dispenser == null)
            {
                throw new ArgumentNullException(nameof(dispenser));
            }

            _context.Dispensers.Remove(dispenser);
        }

        public bool DispenserExists(Guid dispenserId)
        {
            if (dispenserId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(dispenserId));
            }

            return _context.Dispensers.Any(a => a.Id == dispenserId);
        }

        public Dispenser GetDispenser(Guid dispenserId)
        {
            if (dispenserId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(dispenserId));
            }

            return _context.Dispensers.FirstOrDefault(a => a.Id == dispenserId);
        }

        public PagedList<Dispenser> GetDispensers(DispenserResourceParameters dispenserResourseParameters)
        {
            if (dispenserResourseParameters == null)
            {
                throw new ArgumentNullException(nameof(dispenserResourseParameters));
            }

            var collection = _context.Dispensers as IQueryable<Dispenser>;

            if (!string.IsNullOrWhiteSpace(dispenserResourseParameters.Type))
            {
                var type = dispenserResourseParameters.Type.Trim();
                collection = collection.Where(a => a.DispenserType == type);
            }

            if (!string.IsNullOrWhiteSpace(dispenserResourseParameters.SearchQuery))
            {
                var searchQuery = dispenserResourseParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.DispenserType.Contains(searchQuery)
                 || a.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(dispenserResourseParameters.OrderBy))
            {
                var dispenserPropertyMappingDictionary = _propertyMappingService.GetDispenserPropertyMapping<DispenserDto, Dispenser>();

                collection = collection.ApplySort(dispenserResourseParameters.OrderBy, dispenserPropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<Dispenser>.Create(collection,
                dispenserResourseParameters.PageNumber,
                dispenserResourseParameters.PageSize);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateDispenser(Dispenser dispenser)
        {
            if (dispenser == null)
            {
                throw new ArgumentNullException(nameof(dispenser));
            }

            _context.Dispensers.Update(dispenser);
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
