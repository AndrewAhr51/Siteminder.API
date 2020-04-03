using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles 
{
    public class FuelProfile : Profile
    {
        public FuelProfile()
        {
            CreateMap<Entities.Fuel, Models.FuelDto>();

            CreateMap<Models.FuelForCreationDto, Entities.Fuel>();

            CreateMap<Models.FuelForUpdateDto, Entities.Fuel>();

            CreateMap<Models.FuelDto, Entities.Fuel>();

            CreateMap<Entities.Fuel, Models.FuelFullDto>();

            CreateMap<Entities.Fuel, Models.FuelForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.FuelForUpdateDto>, JsonPatchDocument<Entities.Fuel>>();

            CreateMap<Operation<Models.FuelForUpdateDto>, Operation<Entities.Fuel>>();
        }
        
    }
}
