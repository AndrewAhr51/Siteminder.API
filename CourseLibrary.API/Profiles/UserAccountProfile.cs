using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace Siteminder.API.Profiles
{
    public class UserAccountProfile : Profile
    {

        public UserAccountProfile()
        {
            CreateMap<Entities.UserAccount, Models.UserAccountDto>();

            CreateMap<Models.UserAccountForCreationDto, Entities.UserAccount>();

            CreateMap<Models.UserAccountForUpdateDto, Entities.UserAccount>();

            CreateMap<Models.UserAccountDto, Entities.UserAccount>();

            CreateMap<Entities.UserAccount, Models.UserAccountFullDto>();

            CreateMap<Entities.UserAccount, Models.UserAccountForUpdateDto>();

            CreateMap<JsonPatchDocument<Models.UserAccountForUpdateDto>, JsonPatchDocument<Entities.UserAccount>>();

            CreateMap<Operation<Models.UserAccountForUpdateDto>, Operation<Entities.UserAccount>>();
        }
      

    }
}
