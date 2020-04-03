using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class SitesProfileProfile : Profile
    {
        public SitesProfileProfile()
        {
            CreateMap<Entities.Site, Models.SiteDto>();

            CreateMap<Models.SiteForCreationDto, Entities.Site>();

            CreateMap<Models.SiteForUpdateDto, Entities.Site>();
            
            CreateMap<Models.SiteDto, Entities.Site>();

            CreateMap<Entities.Site, Models.SiteFullDto>();

            CreateMap<Entities.Site, Models.SiteForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.SiteForUpdateDto>, JsonPatchDocument<Entities.Site>>();

            CreateMap<Operation<Models.SiteForUpdateDto>, Operation<Entities.Site>>();
        }
    }
}
