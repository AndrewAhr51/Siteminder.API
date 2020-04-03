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
    public class DeviceRepository : IDeviceRepository, IDisposable
    {
        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public DeviceRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddDevice(Device device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            device.Id = Guid.NewGuid();

            _context.Device.Add(device);
        }

        public void AddDevice(Guid deviceId, Device device)
        {
            if (deviceId == null)
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            device.Id = deviceId;

            _context.Device.Add(device);
        }

        public void DeleteDevice(Device device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            _context.Device.Remove(device);
        }

        public bool DeviceExists(Guid deviceId)
        {
            if (deviceId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            return _context.Device.Any(a => a.Id == deviceId);
        }

        public Device GetDevices(Guid terminalId, Guid deviceId)
        {
            if (terminalId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(terminalId));
            }

            if (deviceId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            return _context.Device.Where(a => a.TerminalId == terminalId || a.Id == deviceId).FirstOrDefault();
        }

        public PagedList<Device> GetDevices(DeviceResourceParameters deviceResourseParameters)
        {
            if (deviceResourseParameters == null)
            {
                throw new ArgumentNullException(nameof(deviceResourseParameters));
            }

            if (deviceResourseParameters.TerminalId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(deviceResourseParameters));
            }

            var collection = _context.Device as IQueryable<Device>;

            collection = collection.Where(a => a.TerminalId == deviceResourseParameters.TerminalId);

            if (!string.IsNullOrWhiteSpace(deviceResourseParameters.SearchQuery))
            {
                var searchQuery = deviceResourseParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.ModelName.Contains(searchQuery) || a.SerialNumber.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(deviceResourseParameters.OrderBy))
            {
                var devicePropertyMappingDictionary = _propertyMappingService.GetDevicePropertyMapping<DevicesDto, Device>();

                collection = collection.ApplySort(deviceResourseParameters.OrderBy, devicePropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<Device>.Create(collection,
                deviceResourseParameters.PageNumber,
                deviceResourseParameters.PageSize);
        }

        public void UpdateDevice(Device device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }
            
            _context.Device.Update(device);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
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
