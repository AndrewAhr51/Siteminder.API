using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class ContactProfiles : Profile
    {
        public ContactProfiles()
        {
            //CreateMap<Entities.Contact, Models.ContactDto>()
            //   .ForMember(
            //       dest => dest.Name,
            //       opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<Entities.Contact, Models.ContactDto>();

            CreateMap<Models.ContactForCreationDto, Entities.Contact>();

            CreateMap<Models.SiteContactForCreationDto, Entities.SiteContacts>();

            CreateMap<Models.TerminalContactForCreationDto, Entities.TerminalContacts>();

            CreateMap<Models.ContactForUpdateDto, Entities.Contact>();

            CreateMap<Entities.Contact, Models.ContactFullDto>();

            CreateMap<Entities.SiteContacts, Models.ContactFullDto>();

            CreateMap<Entities.SiteContacts, Models.SiteContactForCreationDto>();

            CreateMap<JsonPatchDocument<Models.ContactForUpdateDto>, JsonPatchDocument<Entities.Contact>>();

            CreateMap<Operation<Models.ContactForUpdateDto>, Operation<Entities.Contact>>();
        }
    }
}

