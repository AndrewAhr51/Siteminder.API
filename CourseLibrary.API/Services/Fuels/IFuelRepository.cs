using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public interface IFuelRepository
    {
        void AddFuel(Fuel fuel);
        void AddFuel(Guid fuelTypeId, Guid fuelId, Fuel fuel);
        void DeleteFuel(Fuel fuel);
        public PagedList<Fuel> GetAllFuel(FuelResourceParameters fuelResourceParameters);
        public PagedList<Fuel> GetFuelByType(Guid fuelTypeId, FuelResourceParameters fuelResourceParameters);
        Fuel GetFuel(Guid fuelId);
        public PagedList<Fuel> GetFuel(FuelResourceParameters fuelResourceParameters);
        void UpdateFuel(Fuel fuel);
        bool FuelExists(Guid fuelId);
        bool Save();
    }
}
