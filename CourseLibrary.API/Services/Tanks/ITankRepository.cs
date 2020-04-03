using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.ResourceParameters;
using System;

namespace Siteminder.API.Services.Tanks
{
    public interface ITankRepository
    {
        void AddTank(Tank tank);
        void AddTank(TankResourceParameters tankParameters, Guid tankId);
        void DeleteTank(Tank tank);
        Tank GetTank(TankResourceParameters tankParameters, Guid tankId);
        public PagedList<Tank> GetTanks(TankResourceParameters tankParameters);
        void UpdateTank(Tank tank);
        bool TankExists(Guid tankId);
        bool Save();
    }
}
