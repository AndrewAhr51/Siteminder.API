using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class TerminalProfile : Profile
    {
        public TerminalProfile()
        {
            CreateMap<Entities.Terminal, Models.TerminalDto>();

            CreateMap<Models.TerminalForCreationDto, Entities.Terminal>();

            CreateMap<Models.TerminalForUpdateDto, Entities.Terminal>();

            CreateMap<Models.TerminalDto, Entities.Terminal>();

            CreateMap<Entities.Terminal, Models.TerminalFullDto>();
            
            CreateMap<Entities.Terminal, Models.TerminalForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.TerminalForUpdateDto>, JsonPatchDocument<Entities.Terminal>>();

            CreateMap<Operation<Models.TerminalForUpdateDto>, Operation<Entities.Terminal>>();

        }

    }
}
