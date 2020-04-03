using System;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class TankProfile : Profile
    {
        public TankProfile()
        {
            CreateMap<Entities.Tank, Models.Tanks.TankDto>();

            CreateMap<Models.Tanks.TankForCreationDto, Entities.Tank>();

            CreateMap<Models.Tanks.TankForUpdateDto, Entities.Tank>();

            CreateMap<Entities.Tank, Models.Tanks.TankFullDto>();

            CreateMap<Entities.Tank, Models.Tanks.TankForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.Tanks.TankForUpdateDto>, JsonPatchDocument<Entities.Tank>>();

            CreateMap<Operation<Models.Tanks.TankForUpdateDto>, Operation<Entities.Tank>>();
        }
    }
}
