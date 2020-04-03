using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class DispenserProfile : Profile
    {

        public DispenserProfile()
        {
            CreateMap<Entities.Dispenser, Models.DispenserDto>();

            CreateMap<Models.DispenserForCreationDto, Entities.Dispenser>();

            CreateMap<Models.DispenserForUpdateDto, Entities.Dispenser>();

            CreateMap<Models.DispenserDto, Entities.Dispenser>();

            CreateMap<Entities.Dispenser, Models.DispenserFullDto>();

            CreateMap<Entities.Dispenser, Models.DispenserFullDto>();

            CreateMap<Entities.Dispenser, Models.DispenserForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.DispenserForUpdateDto>, JsonPatchDocument<Entities.Dispenser>>();

            CreateMap<Operation<Models.DispenserForUpdateDto>, Operation<Entities.Dispenser>>();
        }

        
    }
}
