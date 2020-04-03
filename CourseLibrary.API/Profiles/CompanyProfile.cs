using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            //Source and the Destination mapping...
            CreateMap<Entities.Company, Models.CompanyDto>();

            CreateMap<Models.CompanyForCreationDto, Entities.Company>();

            CreateMap<Models.CompanyForUpdateDto, Entities.Company>();

            CreateMap<Entities.Company, Models.CompanyFullDto>();

            CreateMap<Entities.Company, Models.CompanyForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.CompanyForUpdateDto>, JsonPatchDocument<Entities.Company>>();

            CreateMap<Operation<Models.CompanyForUpdateDto>, Operation<Entities.Company>>();
        }
    }
}
