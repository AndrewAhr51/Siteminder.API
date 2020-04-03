using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Siteminder.API.Models.CustomerAccounts;

namespace Siteminder.API.Profiles
{
    public class CustomerAccountProfile : Profile
    {
        public CustomerAccountProfile()
        {
            //CreateMap<Entities.CustomerAccount, Models.AccountDto>()
            //    .ForMember(
            //        dest => dest.Name,
            //        opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            //;

            CreateMap<Entities.CustomerAccount, Models.CustomerAccounts.CustomerAccountDto >();

            CreateMap<Models.CustomerAccounts.CustomerAccountForCreationDto, Entities.CustomerAccount>();

            CreateMap<Models.CompanyForUpdateDto, Entities.CustomerAccount>();

            CreateMap<Entities.CustomerAccount, Models.CustomerAccounts.CustomerAccountFullDto >();

            CreateMap<Entities.CustomerAccount, Models.CustomerAccounts.CustomerAccountForUpdateDto >();

            CreateMap<JsonPatchDocument< Models.CustomerAccounts.CustomerAccountForUpdateDto >, JsonPatchDocument<Entities.CustomerAccount>>();

            CreateMap<Operation< Models.CustomerAccounts.CustomerAccountForUpdateDto >, Operation<Entities.CustomerAccount>>();
        }
    }
}

