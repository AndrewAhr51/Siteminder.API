using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class SiteTypeProfile : Profile
    {
        public SiteTypeProfile()
        {
            //Source and the Destination mapping...
            CreateMap<Entities.SiteType, Models.SiteTypeDto>();

            CreateMap<Models.SiteTypeForCreationDto, Entities.SiteType>();

            CreateMap<Models.SiteTypeForUpdateDto, Entities.SiteType>();

            CreateMap<Entities.SiteType, Models.SiteTypeFullDto>();

            CreateMap<Entities.SiteType, Models.SiteTypeForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.SiteTypeForUpdateDto>, JsonPatchDocument<Entities.SiteType>>();

            CreateMap<Operation<Models.SiteTypeForUpdateDto>, Operation<Entities.SiteType>>();
        }
    }
}
