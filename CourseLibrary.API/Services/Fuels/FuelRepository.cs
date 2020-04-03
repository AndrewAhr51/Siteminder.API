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
    public class FuelRepository : IFuelRepository, IDisposable
    {
        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public FuelRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddFuel(Fuel fuel)
        {
            if (fuel == null)
            {
                throw new ArgumentNullException(nameof(fuel));
            }

            fuel.Id = Guid.NewGuid();

            _context.Fuel.Add(fuel);
        }

        public void AddFuel(Guid fuelTypeId, Guid fuelId, Fuel fuel)
        {
            if (fuel == null || fuelTypeId == null || fuelId == null)
            {
                throw new ArgumentNullException(nameof(AddFuel));
            }

            fuel.Id = fuelId;
            fuel.FuelTypeId = fuelTypeId;

            _context.Fuel.Add(fuel);
        }

        public void DeleteFuel(Fuel fuel)
        {
            if (fuel == null)
            {
                throw new ArgumentNullException(nameof(fuel));
            }

            _context.Fuel.Remove(fuel);
        }

        public bool FuelExists(Guid fuelId)
        {
            if (fuelId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(fuelId));
            }

            return _context.Fuel.Any(a => a.Id == fuelId);
        }

        public PagedList<Fuel> GetFuel(FuelResourceParameters fuelParameters)
        {
            if (fuelParameters == null)
            {
                throw new ArgumentNullException(nameof(fuelParameters));
            }

            var collection = _context.Fuel as IQueryable<Fuel>;

            if (!string.IsNullOrWhiteSpace(fuelParameters.SearchQuery))
            {
                var searchQuery = fuelParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(fuelParameters.OrderBy))
            {
                var fuelPropertyMappingDictionary = _propertyMappingService.GetFuelPropertyMapping<FuelDto, Fuel>();

                collection = collection.ApplySort(fuelParameters.OrderBy, fuelPropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<Fuel>.Create(collection,
                fuelParameters.PageNumber,
                fuelParameters.PageSize);
        }

        public Fuel GetFuel(Guid fuelId)
        {
            if (fuelId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(fuelId));
            }

            return _context.Fuel.FirstOrDefault(a => a.Id == fuelId); ;
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateFuel(Fuel fuel)
        {
            if (fuel == null)
            {
                throw new ArgumentNullException(nameof(fuel));
            }

            _context.Fuel.Update(fuel);
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

        public PagedList<Fuel> GetAllFuel(FuelResourceParameters fuelResourceParameters)
        {
            if (fuelResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(fuelResourceParameters));
            }

            var collection = _context.Fuel as IQueryable<Fuel>;

            if (!string.IsNullOrWhiteSpace(fuelResourceParameters.SearchQuery))
            {
                var searchQuery = fuelResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(fuelResourceParameters.OrderBy))
            {
                var fuelPropertyMappingDictionary = _propertyMappingService.GetFuelPropertyMapping<FuelDto, Fuel>();

                collection = collection.ApplySort(fuelResourceParameters.OrderBy, fuelPropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<Fuel>.Create(collection,
                fuelResourceParameters.PageNumber,
                fuelResourceParameters.PageSize);
        }

        public PagedList<Fuel> GetFuelByType(Guid fuelTypeId, FuelResourceParameters fuelResourceParameters)
        {
            if (fuelResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(fuelResourceParameters));
            }

            var collection = _context.Fuel as IQueryable<Fuel>;

            if (fuelResourceParameters.FuelType != Guid.Empty)
            {
                collection = collection.Where(a => a.FuelTypeId == fuelTypeId);
            }

            if (!string.IsNullOrWhiteSpace(fuelResourceParameters.SearchQuery))
            {
                var searchQuery = fuelResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(fuelResourceParameters.OrderBy))
            {
                var fuelPropertyMappingDictionary = _propertyMappingService.GetFuelPropertyMapping<FuelDto, Fuel>();

                collection = collection.ApplySort(fuelResourceParameters.OrderBy, fuelPropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<Fuel>.Create(collection,
                fuelResourceParameters.PageNumber,
                fuelResourceParameters.PageSize);
        }
    }
}
