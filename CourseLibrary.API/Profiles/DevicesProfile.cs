using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Siteminder.API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class DevicesProfile: Profile
    {
        public DevicesProfile()
        {
            CreateMap<Entities.Device, Models.DevicesDto>();
            
            CreateMap<Models.DevicesForCreationDto, Entities.Device>();

            CreateMap<Models.DevicesForUpdateDto, Entities.Device>();

            CreateMap<Models.DevicesDto, Entities.Device>();

            CreateMap<Entities.Device, Models.DevicesFullDto>();
           
            CreateMap<Entities.Device, Models.DevicesFullDto>();

            CreateMap<Entities.Device, Models.DevicesForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.DevicesForUpdateDto>, JsonPatchDocument<Entities.Device>>();

            CreateMap<Operation<Models.DevicesForUpdateDto>, Operation<Entities.Device>>();
        }
        
    }
}
