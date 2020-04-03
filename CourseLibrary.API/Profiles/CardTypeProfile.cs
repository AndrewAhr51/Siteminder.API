using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Profiles
{
    public class CardTypeProfile : Profile
    {

        public CardTypeProfile()
        {
            CreateMap<Entities.CardType, Models.CardTypeDto>();

            CreateMap<Models.CardTypeForCreationDto, Entities.CardType>();

            CreateMap<Models.CardTypeForUpdateDto, Entities.CardType>();

            CreateMap<Entities.CardType, Models.CardTypeFullDto>();

            CreateMap<Entities.CardType, Models.CardTypeForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.CardTypeForUpdateDto>, JsonPatchDocument<Entities.CardType>>();

            CreateMap<Operation<Models.CardTypeForUpdateDto>, Operation<Entities.CardType>>();
        }
       
    }
}
