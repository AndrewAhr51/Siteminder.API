using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public interface IFuelTypeRepository
    {
        void AddFuelType(FuelType fuelType);

        void AddFuelType(Guid fuelTypeId, FuelType fuelType);
        void DeleteFuelType(FuelType fuelType);
        FuelType GetFuelType(Guid fuelTypeId);
        public PagedList<FuelType> GetFuelTypes(FuelTypeResourceParameters fuelTypeParameters);
        void UpdateFuelType(FuelType fuelType);
        bool FuelTypeExists(Guid fuelTypeId);
        bool Save();
    }
}
