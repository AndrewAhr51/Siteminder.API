using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Siteminder.API.Services;
using Siteminder.API.Models;
using Siteminder.API.Helper;
using AutoMapper;
using Siteminder.API.ResourceParameters;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using Siteminder.API.Entities;
using Marvin.Cache.Headers;
using Siteminder.API.ActionConstraints;
using Microsoft.AspNetCore.JsonPatch;
using Siteminder.API.Models.CustomerAccounts;

namespace Siteminder.API.Controllers
{

    [ApiController]
    [Route("api/companies/{companyId}/customeraccounts")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class CustomerAccountController : ControllerBase
    {
        private readonly ICustomerAccountRepository _customerAccountRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public CustomerAccountController(ICustomerAccountRepository customerAccountRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _customerAccountRepository = customerAccountRepository ??
               throw new ArgumentNullException(nameof(customerAccountRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [Produces("application/json",
        "application/vnd.marvin.hateoas+json",
        "application/vnd.marvin.getcustomeraccounts.full+json",
        "application/vnd.marvin.getcustomeraccounts.full.hateoas+json",
        "application/vnd.marvin.getcustomeraccounts.friendly+json",
        "application/vnd.marvin.getcustomeraccounts.friendly.hateoas+json")]
        [HttpGet(Name = "GetCustomerAccounts")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        public IActionResult GetCustomerAccounts(Guid companyId,
         [FromQuery] CustomerAccountResourceParameters customerAccountResourceParameters)
        {
            if (!_propertyMappingService.ValidCustomerAccountMappingExistsFor<CustomerAccountDto, Entities.CustomerAccount>
               (customerAccountResourceParameters.OrderBy))
            {
                return BadRequest();
            }
            customerAccountResourceParameters.CompanyId= companyId;

            var customerAccountFromRepo = _customerAccountRepository.GetCustomerAccounts(customerAccountResourceParameters);

            var paginationMetadata = new
            {
                totalCount = customerAccountFromRepo.TotalCount,
                pageSize = customerAccountFromRepo.PageSize,
                currentPage = customerAccountFromRepo.CurrentPage,
                totalPages = customerAccountFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForCustomerAccount(customerAccountResourceParameters,
                customerAccountFromRepo.HasNext,
                customerAccountFromRepo.HasPrevious);

            var shapedCustomerAccount = _mapper.Map<IEnumerable<CustomerAccountDto>>(customerAccountFromRepo)
                               .ShapeData(customerAccountResourceParameters.Fields);

            var shapedCustomerAccountWithLinks = shapedCustomerAccount.Select(customerAccount =>
            {
                var customerAccountAsDictionary = customerAccount as IDictionary<string, object>;
                var customerAccountLinks = CreateLinksForCustomerAccounts(companyId.ToString(), (string)customerAccountAsDictionary["Id"], null);
                customerAccountAsDictionary.Add("links", customerAccountLinks);
                return customerAccountAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedCustomerAccountWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [Produces("application/json",
        "application/vnd.marvin.hateoas+json",
        "application/vnd.marvin.getcustomeraccount.full+json",
        "application/vnd.marvin.getcustomeraccount.full.hateoas+json",
        "application/vnd.marvin.getcustomeraccount.friendly+json",
        "application/vnd.marvin.getcustomeraccount.friendly.hateoas+json")]
        [HttpGet("{customerAccountId}", Name = "GetCustomerAccount")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetCustomerAccount(Guid companyId, Guid customerAccountId, string fields,
        [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<CustomerAccountDto>
               (fields))
            {
                return BadRequest();
            }

            var customerAccountFromRepo = _customerAccountRepository.GetCustomerAccount(companyId, customerAccountId);

            if (customerAccountFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForCustomerAccounts(companyId.ToString(), customerAccountId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full author
            if (primaryMediaType == "vnd.marvin.customeraccount.full")
            {
                var fullResourceToReturn = _mapper.Map<CustomerAccountFullDto>(customerAccountFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly author
            var friendlyResourceToReturn = _mapper.Map<CustomerAccountDto>(customerAccountFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

      
        [HttpPost(Name = "CreateCustomerAccount")]
        [RequestHeaderMatchesMediaType("Content-Type",
          "application/json",
          "application/vnd.marvin.customeraccountforcreation+json")]
        [Consumes("application/json",
          "application/vnd.marvin.customeraccountforcreation+json")]
        public ActionResult<CustomerAccountDto> CreateCustomerAccount(Guid companyId, CustomerAccountForCreationDto customerAccount)
        {
            var customerAccountEntity = _mapper.Map<Entities.CustomerAccount>(customerAccount);
            _customerAccountRepository.CreateCustomerAccount(customerAccountEntity);
            _customerAccountRepository.Save();

            var customerAccountToReturn = _mapper.Map<CustomerAccountDto>(customerAccountEntity);

            var links = CreateLinksForCustomerAccounts(companyId.ToString(), customerAccountToReturn.Id, null);

            var linkedResourceToReturn = customerAccountToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetCustomerAccount",
                new { companyId, CustomerAccountId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [RequestHeaderMatchesMediaType("Content-Type",
          "application/json",
          "application/vnd.marvin.customeraccountforpatch+json")]
        [Consumes("application/json",
          "application/vnd.marvin.customeraccountforpatch+json")]
        [HttpPatch("{customerAccountId}", Name = "PatchCustomerAccount")]
        public ActionResult PatchCustomerAccount(Guid companyId, Guid customerAccountId, [FromBody]JsonPatchDocument<CustomerAccountForUpdateDto> patchDocument)
        {

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_customerAccountRepository.CustomerAccountExists(customerAccountId))
            {
                return NotFound();
            }

            var customerAccountFromRepo = _customerAccountRepository.GetCustomerAccount(companyId, customerAccountId);

            if (customerAccountFromRepo == null)
            {
                var customerAccountDto = new CustomerAccountForUpdateDto();
                patchDocument.ApplyTo(customerAccountDto, ModelState);

                if (!TryValidateModel(customerAccountDto))
                {
                    return ValidationProblem(ModelState);
                }

                var customerAccountToAdd = _mapper.Map<Entities.CustomerAccount>(customerAccountDto);
                customerAccountToAdd.Id = customerAccountId;

                _customerAccountRepository.CreateCustomerAccount(companyId, customerAccountToAdd);

                _customerAccountRepository.Save();

                var companyToReturn = _mapper.Map<CustomerAccountDto>(customerAccountToAdd);

                return CreatedAtRoute("GetCustomerAccount",
                    new { companyId, customerAccountId }, companyToReturn);
            }

            var customerAccountToPatch = _mapper.Map<CustomerAccountForUpdateDto>(customerAccountFromRepo);

            patchDocument.ApplyTo(customerAccountToPatch, ModelState);


            if (!TryValidateModel(customerAccountToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(customerAccountToPatch, customerAccountFromRepo);

            _customerAccountRepository.UpdateCustomerAccount(customerAccountFromRepo);

            _customerAccountRepository.Save();

            return NoContent();

        }

        [HttpDelete("{customerAccountId}", Name = "DeleteCustomerAccount")]
        [RequestHeaderMatchesMediaType("Content-Type",
           "application/json",
           "application/vnd.marvin.customeraccountfordeletion+json")]
        [Consumes("application/json",
           "application/vnd.marvin.customeraccountfordeletion+json")]
        public ActionResult DeleteCustomerAccount(Guid companyId, Guid customerAccountId)
        {
            var customerAccountFromRepo = _customerAccountRepository.GetCustomerAccount(companyId, customerAccountId);

            if (customerAccountFromRepo == null)
            {
                return NotFound();
            }

            _customerAccountRepository.DeleteCustomerAccount(customerAccountFromRepo);

            _customerAccountRepository.Save();

            return NoContent();
        }

        public IActionResult GetCustomerAccountOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PATCH");
            return Ok();
        }
        private string CreateCustomerAccountResourceUri(
          CustomerAccountResourceParameters customerAccountResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCustomerAccounts",
                      new
                      {
                          fields = customerAccountResourceParameters.Fields,
                          orderBy = customerAccountResourceParameters.OrderBy,
                          pageNumber = customerAccountResourceParameters.PageNumber - 1,
                          pageSize = customerAccountResourceParameters.PageSize,
                          mainCategory = customerAccountResourceParameters.Type,
                          searchQuery = customerAccountResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCustomerAccounts",
                      new
                      {
                          fields = customerAccountResourceParameters.Fields,
                          orderBy = customerAccountResourceParameters.OrderBy,
                          pageNumber = customerAccountResourceParameters.PageNumber + 1,
                          pageSize = customerAccountResourceParameters.PageSize,
                          mainCategory = customerAccountResourceParameters.Type,
                          searchQuery = customerAccountResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetCustomerAccounts",
                    new
                    {
                        fields = customerAccountResourceParameters.Fields,
                        orderBy = customerAccountResourceParameters.OrderBy,
                        pageNumber = customerAccountResourceParameters.PageNumber,
                        pageSize = customerAccountResourceParameters.PageSize,
                        mainCategory = customerAccountResourceParameters.Type,
                        searchQuery = customerAccountResourceParameters.SearchQuery
                    });
            }

        }

        private IEnumerable<LinkDto> CreateLinksForCustomerAccounts(string companyId, string customerAccountId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetCustomerAccount", new {companyId,  customerAccountId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetCustomerAccount", new { companyId, customerAccountId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("DeleteCustomerAccount", new { companyId, customerAccountId }),
                   "delete_customer_account",
                   "DELETE"));

                links.Add(
                   new LinkDto(Url.Link("PatchCustomerAccount", new { companyId, customerAccountId }),
                   "patch_customer_account",
                   "PATCH"));
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCustomerAccount(
        CustomerAccountResourceParameters customerAccountResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateCustomerAccountResourceUri(
                   customerAccountResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateCustomerAccountResourceUri(
                      customerAccountResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateCustomerAccountResourceUri(
                        customerAccountResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
