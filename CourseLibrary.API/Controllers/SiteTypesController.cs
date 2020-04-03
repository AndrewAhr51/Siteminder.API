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
    [Route("api/sitetypes")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class SiteTypesController : ControllerBase
    {
        private readonly ISiteTypeRepository _siteTypeRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public SiteTypesController(ISiteTypeRepository siteTypeRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _siteTypeRepository = siteTypeRepository ??
               throw new ArgumentNullException(nameof(siteTypeRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [Produces("application/json",
           "application/vnd.marvin.hateoas+json",
           "application/vnd.marvin.sitetypes.full+json",
           "application/vnd.marvin.sitetypes.full.hateoas+json",
           "application/vnd.marvin.sitetypes.friendly+json",
           "application/vnd.marvin.sitetypes.friendly.hateoas+json")]
        [HttpGet(Name = "GetSiteTypes")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        public IActionResult GetSiteTypes(
            [FromQuery] SiteTypeResourceParameters siteTypeResourceParameters)
        {
            if (!_propertyMappingService.ValidSiteTypeMappingExistsFor<SiteTypeDto, Entities.SiteType>
               (siteTypeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var siteTypesFromRepo = _siteTypeRepository.GetSiteTypes(siteTypeResourceParameters);

            var paginationMetadata = new
            {
                totalCount = siteTypesFromRepo.TotalCount,
                pageSize = siteTypesFromRepo.PageSize,
                currentPage = siteTypesFromRepo.CurrentPage,
                totalPages = siteTypesFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForSiteTypes(siteTypeResourceParameters,
                siteTypesFromRepo.HasNext,
                siteTypesFromRepo.HasPrevious);

            var shapedSiteTypes = _mapper.Map<IEnumerable<SiteTypeDto>>(siteTypesFromRepo)
                               .ShapeData(siteTypeResourceParameters.Fields);

            var shapedSiteTypesWithLinks = shapedSiteTypes.Select(siteType =>
            {
                var siteTypeAsDictionary = siteType as IDictionary<string, object>;
                var siteTypeLinks = CreateLinksForSiteTypes((string)siteTypeAsDictionary["Id"], null);
                siteTypeAsDictionary.Add("links", siteTypeLinks);
                return siteTypeAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedSiteTypesWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [Produces("application/json",
            "application/vnd.marvin.hateoas+json",
            "application/vnd.marvin.sitetypes.full+json",
            "application/vnd.marvin.sitetypes.full.hateoas+json",
            "application/vnd.marvin.sitetypes.friendly+json",
            "application/vnd.marvin.sitetypes.friendly.hateoas+json")]
        [HttpGet("{siteTypeId}", Name = "GetSiteType")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetSiteType(Guid siteTypeId, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<SiteTypeDto>
               (fields))
            {
                return BadRequest();
            }

            var siteTypeFromRepo = _siteTypeRepository.GetSiteType(siteTypeId);

            if (siteTypeFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForSiteTypes(siteTypeId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full author
            if (primaryMediaType == "vnd.marvin.sitetypes.full")
            {
                var fullResourceToReturn = _mapper.Map<SiteTypeFullDto>(siteTypeFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly author
            var friendlyResourceToReturn = _mapper.Map<SiteTypeDto>(siteTypeFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

        [HttpPost(Name = "CreateSiteType")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.sitetypeforcreation+json")]
        [Consumes("application/json",
            "application/vnd.marvin.sitetypeforcreation+json")]
        public ActionResult<SiteTypeDto> CreateSiteType(SiteTypeForCreationDto siteType)
        {
            var siteTypeEntity = _mapper.Map<Entities.SiteType>(siteType);
            _siteTypeRepository.AddSiteType(siteTypeEntity);
            _siteTypeRepository.Save();

            var siteTypeToReturn = _mapper.Map<SiteTypeDto>(siteTypeEntity);

            var links = CreateLinksForSiteTypes(siteTypeToReturn.Id, null);

            var linkedResourceToReturn = siteTypeToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetSiteType",
                new { siteTypeId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [RequestHeaderMatchesMediaType("Content-Type",
       "application/json",
       "application/vnd.marvin.sitetypeforpatch+json")]
        [Consumes("application/json",
       "application/vnd.marvin.sitetypeforpatch+json")]
        [HttpPatch("{siteTypeId}")]
        public ActionResult PatchSiteType(Guid siteTypeId, [FromBody]JsonPatchDocument<SiteTypeForUpdateDto> patchDocument)
        {

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_siteTypeRepository.SiteTypeExists(siteTypeId))
            {
                return NotFound();
            }

            var siteTypeFromRepo = _siteTypeRepository.GetSiteType(siteTypeId);

            if (siteTypeFromRepo == null)
            {
                var siteTypeDto = new SiteTypeForUpdateDto();
                patchDocument.ApplyTo(siteTypeDto, ModelState);

                if (!TryValidateModel(siteTypeDto))
                {
                    return ValidationProblem(ModelState);
                }

                var siteTypeToAdd = _mapper.Map<Entities.SiteType>(siteTypeDto);
                siteTypeToAdd.Id = siteTypeId;

                _siteTypeRepository.AddSiteType(siteTypeId, siteTypeToAdd);

                _siteTypeRepository.Save();

                var sityTypeToReturn = _mapper.Map<SiteTypeDto>(siteTypeToAdd);

                return CreatedAtRoute("GetSiteType",
                    new { siteTypeId }, sityTypeToReturn);
            }

            var siteTypeToPatch = _mapper.Map<SiteTypeForUpdateDto>(siteTypeFromRepo);
            
            patchDocument.ApplyTo(siteTypeToPatch);

            if (!TryValidateModel(siteTypeToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(siteTypeToPatch, siteTypeFromRepo);

            _siteTypeRepository.UpdateSiteType(siteTypeFromRepo);

            _siteTypeRepository.Save();

            return NoContent();

        }


        [HttpPost("{siteTypeId}/company/{companyId}/assignment", Name = "CreateCompanySiteTypeAssignment")]
        [RequestHeaderMatchesMediaType("Content-Type",
         "application/json",
         "application/vnd.marvin.companysitetypeassignmentforcreation+json")]
        [Consumes("application/json",
         "application/vnd.marvin.companysitetypeassignmentforcreation+json")]
        public ActionResult CreateCompanySiteTypeAssignment(Guid companyId, Guid siteTypeId, CompanySiteTypeDto companySiteType)
        {
            var companySiteTypeFromRepo = _siteTypeRepository.GetCompanySiteTypeAssignment(companyId, siteTypeId);

            if (companySiteTypeFromRepo != null)
            {
                return StatusCode(302); //Record Found
            }

            var companysiteTypeEntity = _mapper.Map<Entities.CompanySiteTypes>(companySiteType);
            _siteTypeRepository.CreateCompanySiteTypeAssignment(companysiteTypeEntity);
            _siteTypeRepository.Save();

            return NoContent();
        }

        [HttpDelete("{siteTypeId}/company/{companyId}/unassignment", Name = "DeleteCompanySiteTypeAssignment")]
        [RequestHeaderMatchesMediaType("Content-Type",
           "application/json",
           "application/vnd.marvin.companysitetypeassignmentfordeletion+json")]
        [Consumes("application/json",
           "application/vnd.marvin.companysitetypeassignmentfordeletion+json")]
        public ActionResult DeleteCompanySiteTypeAssignment(Guid companyId, Guid siteTypeId, CompanySiteTypeDto companySiteType)
        {
            var companySiteTypeFromRepo = _siteTypeRepository.GetCompanySiteTypeAssignment(companyId, siteTypeId);

            if (companySiteTypeFromRepo == null)
            {
                return NotFound();
            }

            //var companysiteTypeEntity = _mapper.Map<Entities.CompanySiteTypes>(companySiteType);
            _siteTypeRepository.DeleteCompanySiteTypeAssignment(companySiteTypeFromRepo);
            _siteTypeRepository.Save();

            return NoContent();
        }

        [HttpDelete("{siteTypeId}", Name = "DeleteSiteType")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.sitetypefordeletion+json")]
        [Consumes("application/json",
            "application/vnd.marvin.sitetypefordeletion+json")]
        public ActionResult DeleteSiteType(Guid siteTypeId)
        {
            var siteTypeFromRepo = _siteTypeRepository.GetSiteType(siteTypeId);

            if (siteTypeFromRepo == null)
            {
                return NotFound();
            }

            _siteTypeRepository.DeleteSiteType(siteTypeFromRepo);

            _siteTypeRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetSiteTypeOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
        private string CreateSiteTypesResourceUri(
          SiteTypeResourceParameters siteTypeResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetSiteTypes",
                      new
                      {
                          fields = siteTypeResourceParameters.Fields,
                          orderBy = siteTypeResourceParameters.OrderBy,
                          pageNumber = siteTypeResourceParameters.PageNumber - 1,
                          pageSize = siteTypeResourceParameters.PageSize,
                          searchQuery = siteTypeResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetSiteTypes",
                      new
                      {
                          fields = siteTypeResourceParameters.Fields,
                          orderBy = siteTypeResourceParameters.OrderBy,
                          pageNumber = siteTypeResourceParameters.PageNumber + 1,
                          pageSize = siteTypeResourceParameters.PageSize,
                          searchQuery = siteTypeResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetSiteTypes",
                    new
                    {
                        fields = siteTypeResourceParameters.Fields,
                        orderBy = siteTypeResourceParameters.OrderBy,
                        pageNumber = siteTypeResourceParameters.PageNumber,
                        pageSize = siteTypeResourceParameters.PageSize,
                        searchQuery = siteTypeResourceParameters.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForSiteTypes(string siteTypeId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetSiteTypes", new { siteTypeId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetSiteType", new { siteTypeId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("DeleteSiteType", new { siteTypeId }),
                   "delete_sitetype",
                   "DELETE"));

                links.Add(
                   new LinkDto(Url.Link("PatchSiteType", new { siteTypeId }),
                   "patch_sitetype",
                   "PATCH"));
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForSiteTypes(
        SiteTypeResourceParameters siteTypeResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateSiteTypesResourceUri(
                   siteTypeResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateSiteTypesResourceUri(
                      siteTypeResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateSiteTypesResourceUri(
                        siteTypeResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
