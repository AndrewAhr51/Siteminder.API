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
    [Route("api/companies/{companyId}/sitetypes/{siteTypeId}")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class SitesController : ControllerBase
    {
        private readonly ISiteRepository _siteRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public SitesController(ISiteRepository siteRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _siteRepository = siteRepository ??
               throw new ArgumentNullException(nameof(siteRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }


        [Produces("application/json",
          "application/vnd.marvin.hateoas+json",
          "application/vnd.marvin.getsites.full+json",
          "application/vnd.marvin.getsites.full.hateoas+json",
          "application/vnd.marvin.getsites.friendly+json",
          "application/vnd.marvin.getsites.friendly.hateoas+json")]
        [HttpGet("sites/sites", Name = "GetSites")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        public IActionResult GetSites(Guid companyId, Guid siteTypeId,
           [FromQuery] SiteResourceParameters siteResourceParameters)
        {
            if (!_propertyMappingService.ValidSiteMappingExistsFor<SiteDto, Entities.Site>
               (siteResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            siteResourceParameters.CompanyId = companyId;

            var sitesFromRepo = _siteRepository.GetSites(siteResourceParameters);

            var paginationMetadata = new
            {
                totalCount = sitesFromRepo.TotalCount,
                pageSize = sitesFromRepo.PageSize,
                currentPage = sitesFromRepo.CurrentPage,
                totalPages = sitesFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForSite(siteResourceParameters,
                sitesFromRepo.HasNext,
                sitesFromRepo.HasPrevious);

            var shapedSites = _mapper.Map<IEnumerable<SiteDto>>(sitesFromRepo)
                               .ShapeData(siteResourceParameters.Fields);

            var shapedSitesWithLinks = shapedSites.Select(sites =>
            {
                var siteAsDictionary = sites as IDictionary<string, object>;
                var siteLinks = CreateLinksForSite(companyId.ToString(), siteTypeId.ToString(), (string)siteAsDictionary["Id"], null);
                siteAsDictionary.Add("links", siteLinks);
                return siteAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedSitesWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [Produces("application/json",
          "application/vnd.marvin.hateoas+json",
          "application/vnd.marvin.getsiteforcompany.full+json",
          "application/vnd.marvin.getsiteforcompany.full.hateoas+json",
          "application/vnd.marvin.getsiteforcompany.friendly+json",
          "application/vnd.marvin.getsiteforcompany.friendly.hateoas+json")]
        [HttpGet("sites/{siteId}", Name = "GetSite")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.getsiteforcompany+json")]
        [Consumes("application/json",
            "application/vnd.marvin.getsiteforcompany+json")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetSite(Guid companyId,Guid siteTypeId, Guid siteId, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<SiteDto>
               (fields))
            {
                return BadRequest();
            }

            var siteFromRepo = _siteRepository.GetSite(companyId, siteTypeId, siteId);

            if (siteFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForSite(companyId.ToString(), siteTypeId.ToString(),siteId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full Site
            if (primaryMediaType == "vnd.marvin.site.full")
            {
                var fullResourceToReturn = _mapper.Map<SiteFullDto>(siteFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly site
            var friendlyResourceToReturn = _mapper.Map<SiteDto>(siteFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

        [HttpPost("sites", Name = "CreateSiteForCompany")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.createsiteforcompany+json")]
        [Consumes("application/json",
            "application/vnd.marvin.createsiteforcompany+json")]
        public ActionResult<SiteDto> CreateSiteForCompany(Guid companyId, Guid siteTypeId, SiteForCreationDto site)
        {

            var siteEntity = _mapper.Map<Entities.Site>(site);
            siteEntity.CompanyId = companyId;
            siteEntity.SiteTypeId = siteTypeId;

            _siteRepository.AddSite(siteEntity);
            _siteRepository.Save();

            var siteToReturn = _mapper.Map<SiteDto>(siteEntity);

            var links = CreateLinksForSite(companyId.ToString(), siteTypeId.ToString(), siteToReturn.Id, null);

            var linkedResourceToReturn = siteToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetSite",
                new { companyId, siteTypeId, siteId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.siteforpatch+json")]
        [Consumes("application/json",
           "application/vnd.marvin.siteforpatch+json")]
        [HttpPatch("sites/{siteId}", Name = "PatchSite")]
        public ActionResult PatchSite(Guid companyId, Guid siteTypeId, Guid siteId, [FromBody]JsonPatchDocument<SiteForUpdateDto> patchDocument)
        {

            if (companyId == null || siteTypeId == null || siteId == null)
            {
                return BadRequest();
            }

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_siteRepository.SiteExists(siteId))
            {
                return NotFound();
            }

            var siteFromRepo = _siteRepository.GetSite(companyId, siteTypeId, siteId);

            if (siteFromRepo == null)
            {
                var siteDto = new SiteForUpdateDto();
                patchDocument.ApplyTo(siteDto, ModelState);

                if (!TryValidateModel(siteDto))
                {
                    return ValidationProblem(ModelState);
                }

                var siteToAdd = _mapper.Map<Entities.Site>(siteDto);
                siteToAdd.CompanyId = companyId;

                _siteRepository.AddSite(siteId, siteToAdd);

                _siteRepository.Save();

                var siteToReturn = _mapper.Map<SiteDto>(siteToAdd);

                return CreatedAtRoute("GetSite",
                    new { siteId }, siteToReturn);
            }

            var siteToPatch = _mapper.Map<SiteForUpdateDto>(siteFromRepo);

            patchDocument.ApplyTo(siteToPatch);

            if (!TryValidateModel(siteToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(siteToPatch, siteFromRepo);

            _siteRepository.UpdateSite(siteFromRepo);

            _siteRepository.Save();

            return NoContent();
        }

        [HttpDelete("sites/{siteId}", Name = "DeleteSite")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.deletesite+json")]
        [Consumes("application/json",
            "application/vnd.marvin.deletesite+json")]
        public ActionResult DeleteSite(Guid companyId, Guid siteTypeId, Guid siteId)
        {
            var siteFromRepo = _siteRepository.GetSite(companyId, siteTypeId, siteId);

            if (siteFromRepo == null)
            {
                return NotFound();
            }

            _siteRepository.DeleteSite(siteFromRepo);

            _siteRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetSiteOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
        private string CreateSitesResourceUri(
          SiteResourceParameters siteResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetSites",
                      new
                      {
                          fields = siteResourceParameters.Fields,
                          orderBy = siteResourceParameters.OrderBy,
                          pageNumber = siteResourceParameters.PageNumber - 1,
                          pageSize = siteResourceParameters.PageSize,
                          searchQuery = siteResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetSites",
                      new
                      {
                          fields = siteResourceParameters.Fields,
                          orderBy = siteResourceParameters.OrderBy,
                          pageNumber = siteResourceParameters.PageNumber + 1,
                          pageSize = siteResourceParameters.PageSize,
                          searchQuery = siteResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetSites",
                    new
                    {
                        fields = siteResourceParameters.Fields,
                        orderBy = siteResourceParameters.OrderBy,
                        pageNumber = siteResourceParameters.PageNumber,
                        pageSize = siteResourceParameters.PageSize,
                        searchQuery = siteResourceParameters.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForSite(string companyId, string siteTypeId, string siteId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetSite", new { companyId, siteTypeId, siteId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetSite", new { companyId, siteTypeId, siteId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("PatchSite", new { companyId, siteTypeId, siteId }),
                   "patch_site",
                   "PATCH"));

                links.Add(
                   new LinkDto(Url.Link("DeleteSite", new { companyId, siteTypeId, siteId }),
                   "delete_site",
                   "DELETE"));

            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForSite(
        SiteResourceParameters siteResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateSitesResourceUri(
                   siteResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateSitesResourceUri(
                      siteResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateSitesResourceUri(
                        siteResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
