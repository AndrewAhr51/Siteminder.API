using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class TerminalSettingsProfile : Profile
    {
        public TerminalSettingsProfile()
        {
            CreateMap<Entities.TerminalSettings, Models.TerminalSettingsDto>();

            CreateMap<Models.TerminalSettingsDto, Entities.TerminalSettings>();

            CreateMap<Models.TerminalSettingsForUpdateDto, Entities.TerminalSettings>();

            CreateMap<Models.TerminalSettingsForCreationDto, Entities.TerminalSettings>();

            CreateMap<Entities.TerminalSettings, Models.TerminalSettingsFullDto>();

            CreateMap<Entities.TerminalSettings, Models.TerminalSettingsForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.TerminalSettingsForUpdateDto>, JsonPatchDocument<Entities.TerminalSettings>>();

            CreateMap<Operation<Models.TerminalSettingsForUpdateDto>, Operation<Entities.TerminalSettings>>();
        }
    }
}
