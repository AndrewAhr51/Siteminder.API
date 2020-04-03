using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public interface IDispenserRepository
    {
        void AddDispenser(Dispenser dispenser);
        void AddDispenser(Guid dispenserId, Dispenser dispenser);
        void DeleteDispenser(Dispenser dispenser);
        Dispenser GetDispenser(Guid dispenserId);
        public PagedList<Dispenser> GetDispensers(DispenserResourceParameters dispenserResourseParameters);
        void UpdateDispenser(Dispenser dispenser);
        bool DispenserExists(Guid dispenserId);
        bool Save();
    }
}
