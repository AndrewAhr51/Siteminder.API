using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Siteminder.API.Services;
using Siteminder.API.Models;
using Siteminder.API.Helper;
using AutoMapper;
using Siteminder.API.ResourceParameters;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using Marvin.Cache.Headers;
using Siteminder.API.ActionConstraints;
using Microsoft.AspNetCore.JsonPatch;

namespace Siteminder.API.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId}/useraccounts")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public UserAccountController(IUserAccountRepository userAccountRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
          IPropertyCheckerService propertyCheckerService)
        {
            _userAccountRepository = userAccountRepository ??
               throw new ArgumentNullException(nameof(userAccountRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [Produces("application/json",
        "application/vnd.marvin.hateoas+json",
        "application/vnd.marvin.getuseraccounts.full+json",
        "application/vnd.marvin.getuseraccounts.full.hateoas+json",
        "application/vnd.marvin.getuseraccounts.friendly+json",
        "application/vnd.marvin.getuseraccounts.friendly.hateoas+json")]
        [HttpGet(Name = "GetUserAccounts")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        public IActionResult GetUserAccounts(Guid companyId,
         [FromQuery] UserAccountResourceParameters userAccountResourceParameters)
        {
            if (!_propertyMappingService.ValidUserAccountMappingExistsFor<UserAccountDto, Entities.UserAccount>
               (userAccountResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            userAccountResourceParameters.CompanyId = companyId;
            var userAccountFromRepo = _userAccountRepository.GetUserAccounts(userAccountResourceParameters);

            var paginationMetadata = new
            {
                totalCount = userAccountFromRepo.TotalCount,
                pageSize = userAccountFromRepo.PageSize,
                currentPage = userAccountFromRepo.CurrentPage,
                totalPages = userAccountFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForUserAccount(userAccountResourceParameters,
                userAccountFromRepo.HasNext,
                userAccountFromRepo.HasPrevious);

            var shapedUserAccounts = _mapper.Map<IEnumerable<UserAccountDto>>(userAccountFromRepo)
                               .ShapeData(userAccountResourceParameters.Fields);

            var shapedUserAccountsWithLinks = shapedUserAccounts.Select(userAccount =>
            {
                var userAccountAsDictionary = userAccount as IDictionary<string, object>;
                var userAccountLinks = CreateLinksForUserAccount(userAccountResourceParameters.CompanyId.ToString(), userAccountAsDictionary["Id"].ToString(), null);
                userAccountAsDictionary.Add("links", userAccountLinks);
                return userAccountAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedUserAccountsWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [Produces("application/json",
        "application/vnd.marvin.hateoas+json",
        "application/vnd.marvin.getuseraccount.full+json",
        "application/vnd.marvin.getuseraccount.full.hateoas+json",
        "application/vnd.marvin.getuseraccount.friendly+json",
        "application/vnd.marvin.getuseraccount.friendly.hateoas+json")]
        [HttpGet("{userAccountId}", Name = "GetUserAccount")]
        [RequestHeaderMatchesMediaType("Content-Type",
          "application/json",
          "application/vnd.marvin.getuseraccount+json")]
        [Consumes("application/json",
          "application/vnd.marvin.getuseraccount+json")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetUserAccount(Guid companyId, Guid userAccountId, string fields,
          [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<UserAccountDto>
               (fields))
            {
                return BadRequest();
            }

            var userAccountFromRepo = _userAccountRepository.GetUserAccount(companyId, userAccountId);

            if (userAccountFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForUserAccount(companyId.ToString(), userAccountId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full Site
            if (primaryMediaType == "vnd.marvin.useraccount.full")
            {
                var fullResourceToReturn = _mapper.Map<UserAccountFullDto>(userAccountFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly site
            var friendlyResourceToReturn = _mapper.Map<UserAccountDto>(userAccountFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }


        [HttpPost(Name = "CreateUserAccount")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.createuseraccount+json")]
        [Consumes("application/json",
            "application/vnd.marvin.createuseraccount+json")]
        public ActionResult<UserAccountDto> CreateUserAccount(Guid companyId, Guid userAccountId, UserAccountForCreationDto userAccount)
        {

            var userAccountEntity = _mapper.Map<Entities.UserAccount>(userAccount);
            userAccountEntity.CompanyId = companyId;
            userAccountEntity.Id = userAccountId;

            _userAccountRepository.AddUserAccount(userAccountEntity);
            _userAccountRepository.Save();

            var userAccountToReturn = _mapper.Map<UserAccountDto>(userAccountEntity);

            var links = CreateLinksForUserAccount(companyId.ToString(), userAccountToReturn.Id.ToString(), null);

            var linkedResourceToReturn = userAccountToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetUserAccount",
                new { companyId, userAccountId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [RequestHeaderMatchesMediaType("Content-Type",
         "application/json",
         "application/vnd.marvin.useraccountforpatch+json")]
        [Consumes("application/json",
        "application/vnd.marvin.useraccountforpatch+json")]
        [HttpPatch("{userAccountId}", Name = "PatchUserAccount")]
        public ActionResult PatchUserAccount(Guid companyId, Guid userAccountId, [FromBody]JsonPatchDocument<UserAccountForUpdateDto> patchDocument)
        {

            if (companyId == null || userAccountId == null)
            {
                return BadRequest();
            }

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_userAccountRepository.UserAccountExists(userAccountId))
            {
                return NotFound();
            }

            var userAccountFromRepo = _userAccountRepository.GetUserAccount(companyId, userAccountId);

            if (userAccountFromRepo == null)
            {
                var userAccountDto = new UserAccountForUpdateDto();
                patchDocument.ApplyTo(userAccountDto, ModelState);

                if (!TryValidateModel(userAccountDto))
                {
                    return ValidationProblem(ModelState);
                }

                var userAccountToAdd = _mapper.Map<Entities.UserAccount>(userAccountDto);
                userAccountToAdd.CompanyId = companyId;

                _userAccountRepository.AddUserAccount(userAccountId, userAccountToAdd);

                _userAccountRepository.Save();

                var userAccountToReturn = _mapper.Map<UserAccountDto>(userAccountToAdd);

                return CreatedAtRoute("GetUserAccount",
                    new { userAccountId }, userAccountToReturn);
            }

            var userAccountToPatch = _mapper.Map<UserAccountForUpdateDto>(userAccountFromRepo);

            patchDocument.ApplyTo(userAccountToPatch);

            if (!TryValidateModel(userAccountToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(userAccountToPatch, userAccountFromRepo);

            _userAccountRepository.PatchUserAccount(userAccountFromRepo);

            _userAccountRepository.Save();

            return NoContent();
        }


        [HttpDelete("{userAccountId}", Name = "DeleteUserAccount")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.deleteuseraccount+json")]
        [Consumes("application/json",
            "application/vnd.marvin.deleteuseraccount+json")]
        public ActionResult DeleteUserAccount(Guid companyId, Guid userAccountId)
        {
            var userAccountFromRepo = _userAccountRepository.GetUserAccount(companyId, userAccountId);

            if (userAccountFromRepo == null)
            {
                return NotFound();
            }

            _userAccountRepository.DeleteUserAccount(userAccountFromRepo);

            _userAccountRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetUserAccountOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PATCH");
            return Ok();
        }
        private string CreateUserAccountResourceUri(
          UserAccountResourceParameters userAccountResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetUserAccounts",
                      new
                      {
                          fields = userAccountResourceParameters.Fields,
                          orderBy = userAccountResourceParameters.OrderBy,
                          pageNumber = userAccountResourceParameters.PageNumber - 1,
                          pageSize = userAccountResourceParameters.PageSize,
                          searchQuery = userAccountResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetUserAccounts",
                      new
                      {
                          fields = userAccountResourceParameters.Fields,
                          orderBy = userAccountResourceParameters.OrderBy,
                          pageNumber = userAccountResourceParameters.PageNumber + 1,
                          pageSize = userAccountResourceParameters.PageSize,
                          searchQuery = userAccountResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetUserAccounts",
                    new
                    {
                        fields = userAccountResourceParameters.Fields,
                        orderBy = userAccountResourceParameters.OrderBy,
                        pageNumber = userAccountResourceParameters.PageNumber,
                        pageSize = userAccountResourceParameters.PageSize,
                        searchQuery = userAccountResourceParameters.SearchQuery
                    });
            }

        }

        private IEnumerable<LinkDto> CreateLinksForUserAccount(string companyId, string userAccountId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetUserAccount", new { companyId, userAccountId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetUserAccount", new { companyId, userAccountId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("DeleteUserAccount", new { companyId,userAccountId }),
                   "delete_useraccount",
                   "DELETE"));

                links.Add(
                   new LinkDto(Url.Link("PatchUserAccount", new { companyId,userAccountId }),
                   "patch_useraccount",
                   "PATCH"));
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForUserAccount(
        UserAccountResourceParameters userAccountResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateUserAccountResourceUri(
                   userAccountResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateUserAccountResourceUri(
                      userAccountResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateUserAccountResourceUri(
                        userAccountResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}