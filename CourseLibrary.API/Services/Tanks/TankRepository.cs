using Siteminder.API.DbContexts;
using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.Models.Tanks;
using Siteminder.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services.Tanks
{
    public class TankRepository : ITankRepository, IDisposable
    {

        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public TankRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddTank(Tank tank)
        {
            if (tank == null)
            {
                throw new ArgumentNullException(nameof(tank));
            }

            tank.Id = Guid.NewGuid();

            _context.Tanks.Add(tank);
        }

        public void DeleteTank(Tank tank)
        {
            if (tank == null)
            {
                throw new ArgumentNullException(nameof(tank));
            }

            _context.Tanks.Remove(tank);
        }

        public bool TankExists(Guid tankId)
        {
            if (tankId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(tankId));
            }

            return _context.Tanks.Any(a => a.Id == tankId);
        }

        public Tank GetTank(TankResourceParameters tankParameters, Guid tankId)
        {
            if (tankId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(tankId));
            }

            return _context.Tanks.FirstOrDefault(a => a.Id == tankId); ;
        }

        public PagedList<Tank> GetTanks(TankResourceParameters tankParameters)
        {
            if (tankParameters == null)
            {
                throw new ArgumentNullException(nameof(tankParameters));
            }

            var collection = _context.Tanks as IQueryable<Tank>;

            if (!string.IsNullOrWhiteSpace(tankParameters.SearchQuery))
            {
                var searchQuery = tankParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(tankParameters.OrderBy))
            {
                var tankPropertyMappingDictionary = _propertyMappingService.GetFuelTypePropertyMapping<TankDto, Tank>();

                collection = collection.ApplySort(tankParameters.OrderBy, tankPropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<Tank>.Create(collection,
                tankParameters.PageNumber,
                tankParameters.PageSize);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }


        public void UpdateTank(Tank tank)
        {
            if (tank == null)
            {
                throw new ArgumentNullException(nameof(tank));
            }

            _context.Tanks.Update(tank);
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
