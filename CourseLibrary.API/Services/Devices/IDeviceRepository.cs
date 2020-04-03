using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public interface IDeviceRepository
    {
        void AddDevice(Device device);
        void AddDevice(Guid deviceId, Device device);
        void DeleteDevice(Device device);
        Device GetDevices(Guid terminalId, Guid deviceId);
        public PagedList<Device> GetDevices(DeviceResourceParameters deviceResourseParameters);
        void UpdateDevice(Device device);
        bool DeviceExists(Guid deviceId);
        bool Save();
    }
}
