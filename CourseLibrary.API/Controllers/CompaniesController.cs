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

namespace Siteminder.API.Controllers
{
    [ApiController]
    [Route("api/companies")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public CompaniesController(ICompanyRepository companyRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _companyRepository = companyRepository ??
               throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [Produces("application/json",
           "application/vnd.marvin.hateoas+json",
           "application/vnd.marvin.company.full+json",
           "application/vnd.marvin.company.full.hateoas+json",
           "application/vnd.marvin.company.friendly+json",
           "application/vnd.marvin.company.friendly.hateoas+json")]
        [HttpGet(Name = "GetCompanies")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        public IActionResult GetCompanies(
            [FromQuery] CompanyResourceParameters companyResourceParameters)
        {
            if (!_propertyMappingService.ValidCompanyMappingExistsFor<CompanyDto, Entities.Company>
               (companyResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var companiesFromRepo = _companyRepository.GetCompanies(companyResourceParameters);

            var paginationMetadata = new
            {
                totalCount = companiesFromRepo.TotalCount,
                pageSize = companiesFromRepo.PageSize,
                currentPage = companiesFromRepo.CurrentPage,
                totalPages = companiesFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForCompany(companyResourceParameters,
                companiesFromRepo.HasNext,
                companiesFromRepo.HasPrevious);

            var shapedCompanies = _mapper.Map<IEnumerable<CompanyDto>>(companiesFromRepo)
                               .ShapeData(companyResourceParameters.Fields);

            var shapedCompaniesWithLinks = shapedCompanies.Select(company =>
            {
                var companyAsDictionary = company as IDictionary<string, object>;
                var companyLinks = CreateLinksForCompany((string)companyAsDictionary["Id"], null);
                companyAsDictionary.Add("links", companyLinks);
                return companyAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedCompaniesWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [Produces("application/json",
            "application/vnd.marvin.hateoas+json",
            "application/vnd.marvin.company.full+json",
            "application/vnd.marvin.company.full.hateoas+json",
            "application/vnd.marvin.company.friendly+json",
            "application/vnd.marvin.company.friendly.hateoas+json")]
        [HttpGet("{companyId}", Name = "GetCompany")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetCompany(Guid companyId, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>
               (fields))
            {
                return BadRequest();
            }

            var companyFromRepo = _companyRepository.GetCompany(companyId);

            if (companyFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForCompany(companyId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full author
            if (primaryMediaType == "vnd.marvin.company.full")
            {
                var fullResourceToReturn = _mapper.Map<CompanyFullDto>(companyFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly author
            var friendlyResourceToReturn = _mapper.Map<CompanyDto>(companyFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

        [HttpPost (Name ="CreateCompany")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.companyforcreation+json")]
        [Consumes("application/json",
            "application/vnd.marvin.companyforcreation+json")]
        public ActionResult<CompanyDto> CreateCompany(CompanyForCreationDto company)
        {
            var companyEntity = _mapper.Map<Entities.Company>(company);
            _companyRepository.AddCompany(companyEntity);
            _companyRepository.Save();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

            var links = CreateLinksForCompany(companyToReturn.Id, null);

            var linkedResourceToReturn = companyToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetCompany",
                new { companyId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.companyforpatch+json")]
        [Consumes("application/json",
            "application/vnd.marvin.companyforpatch+json")]
        [HttpPatch("{companyId}", Name="PatchCompany")]
        public ActionResult PatchCompany(Guid companyId, [FromBody]JsonPatchDocument<CompanyForUpdateDto> patchDocument)
        {

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_companyRepository.CompanyExists(companyId))
            {
                return NotFound();
            }

            var companyFromRepo = _companyRepository.GetCompany(companyId);
            
            if (companyFromRepo == null)
            {
                var companyDto = new CompanyForUpdateDto();
                patchDocument.ApplyTo(companyDto, ModelState);

                if (!TryValidateModel(companyDto))
                {
                    return ValidationProblem(ModelState);
                }

                var companyToAdd = _mapper.Map<Entities.Company>(companyDto);
                companyToAdd.Id = companyId;

                _companyRepository.AddCompany(companyId, companyToAdd);

                _companyRepository.Save();

                var companyToReturn = _mapper.Map<CompanyDto>(companyToAdd);

                return CreatedAtRoute("GetCompany",
                    new { companyId}, companyToReturn);
            }

            var companyToPatch = _mapper.Map<CompanyForUpdateDto>(companyFromRepo);

            patchDocument.ApplyTo(companyToPatch, ModelState);
            //patchDocument.ApplyTo(companyToPatch);

            if (!TryValidateModel(companyToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(companyToPatch, companyFromRepo);

            _companyRepository.UpdateCompany(companyFromRepo);

            _companyRepository.Save();

            return NoContent();

        }

        [HttpDelete("{companyId}", Name = "DeleteCompany")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.companyfordeletion+json")]
        [Consumes("application/json",
            "application/vnd.marvin.companyfordeletion+json")]
        public ActionResult DeleteCompany(Guid companyId)
        {
            var companyFromRepo = _companyRepository.GetCompany(companyId);

            if (companyFromRepo == null)
            {
                return NotFound();
            }

            _companyRepository.DeleteCompany(companyFromRepo);

            _companyRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCompanyOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
        private string CreateCompaniesResourceUri(
          CompanyResourceParameters companyResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCompanies",
                      new
                      {
                          fields = companyResourceParameters.Fields,
                          orderBy = companyResourceParameters.OrderBy,
                          pageNumber = companyResourceParameters.PageNumber - 1,
                          pageSize = companyResourceParameters.PageSize,
                          mainCategory = companyResourceParameters.Type,
                          searchQuery = companyResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCompanies",
                      new
                      {
                          fields = companyResourceParameters.Fields,
                          orderBy = companyResourceParameters.OrderBy,
                          pageNumber = companyResourceParameters.PageNumber + 1,
                          pageSize = companyResourceParameters.PageSize,
                          mainCategory = companyResourceParameters.Type,
                          searchQuery = companyResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetCompanies",
                    new
                    {
                        fields = companyResourceParameters.Fields,
                        orderBy = companyResourceParameters.OrderBy,
                        pageNumber = companyResourceParameters.PageNumber,
                        pageSize = companyResourceParameters.PageSize,
                        mainCategory = companyResourceParameters.Type,
                        searchQuery = companyResourceParameters.SearchQuery
                    });
            }

        }

        private IEnumerable<LinkDto> CreateLinksForCompany(string companyId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetCompany", new { companyId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetCompany", new { companyId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("DeleteCompany", new { companyId }),
                   "delete_company",
                   "DELETE"));
                
                links.Add(
                   new LinkDto(Url.Link("PatchCompany", new { companyId }),
                   "patch_company",
                   "PATCH"));
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(
        CompanyResourceParameters companyResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateCompaniesResourceUri(
                   companyResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateCompaniesResourceUri(
                      companyResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateCompaniesResourceUri(
                        companyResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
