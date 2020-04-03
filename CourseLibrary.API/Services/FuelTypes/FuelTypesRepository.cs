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
    public class FuelTypesRepository : IFuelTypeRepository, IDisposable
    {
        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public FuelTypesRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }
        public void AddFuelType(FuelType fuelType)
        {
            if (fuelType == null)
            {
                throw new ArgumentNullException(nameof(fuelType));
            }

            fuelType.Id = Guid.NewGuid();

            _context.FuelType.Add(fuelType);
        }

        public void AddFuelType(Guid fuelTypeId, FuelType fuelType)
        {
            if (fuelType == null || fuelTypeId == null)
            {
                throw new ArgumentNullException(nameof(AddFuelType));
            }

            fuelType.Id = fuelTypeId;

            _context.FuelType.Add(fuelType);
        }


        public void DeleteFuelType(FuelType fuelType)
        {
            if (fuelType == null)
            {
                throw new ArgumentNullException(nameof(fuelType));
            }

            _context.FuelType.Remove(fuelType);
        }

        public bool FuelTypeExists(Guid fuelTypeId)
        {
            if (fuelTypeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(fuelTypeId));
            }

            return _context.FuelType.Any(a => a.Id == fuelTypeId);
        }

        public FuelType GetFuelType(Guid fuelTypeId)
        {
            if (fuelTypeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(fuelTypeId));
            }

            return _context.FuelType.FirstOrDefault(a => a.Id == fuelTypeId); ;
        }

        public PagedList<FuelType> GetFuelTypes(FuelTypeResourceParameters fuelTypeParameters)
        {
            if (fuelTypeParameters == null)
            {
                throw new ArgumentNullException(nameof(fuelTypeParameters));
            }

            var collection = _context.FuelType as IQueryable<FuelType>;

            if (!string.IsNullOrWhiteSpace(fuelTypeParameters.SearchQuery))
            {
                var searchQuery = fuelTypeParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(fuelTypeParameters.OrderBy))
            {
                var fuelTypePropertyMappingDictionary = _propertyMappingService.GetFuelTypePropertyMapping<FuelTypeDto, FuelType>();

                collection = collection.ApplySort(fuelTypeParameters.OrderBy, fuelTypePropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<FuelType>.Create(collection,
                fuelTypeParameters.PageNumber,
                fuelTypeParameters.PageSize);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateFuelType(FuelType fuelType)
        {
            if (fuelType == null)
            {
                throw new ArgumentNullException(nameof(fuelType));
            }

            _context.FuelType.Update(fuelType);
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
