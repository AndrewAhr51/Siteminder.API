using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class FuelTypeProfile : Profile
    {
        public FuelTypeProfile()
        {
            CreateMap<Entities.FuelType, Models.FuelTypeDto>();

            CreateMap<Models.FuelTypeForCreationDto, Entities.FuelType>();

            CreateMap<Models.FuelTypeForUpdateDto, Entities.FuelType>();

            CreateMap<Models.FuelTypeDto, Entities.FuelType>();

            CreateMap<Entities.FuelType, Models.FuelTypeFullDto>();

            CreateMap<Entities.FuelType, Models.FuelTypeForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.FuelTypeForUpdateDto>, JsonPatchDocument<Entities.FuelType>>();

            CreateMap<Operation<Models.FuelTypeForUpdateDto>, Operation<Entities.FuelType>>();
        }
        
    }
}
