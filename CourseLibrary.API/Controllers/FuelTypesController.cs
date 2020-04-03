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
    [Route("api/fueltypes")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class FuelTypesController : ControllerBase
    {

        private readonly IFuelTypeRepository _fuelTypeRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public FuelTypesController(IFuelTypeRepository fuelTypeRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _fuelTypeRepository = fuelTypeRepository ??
             throw new ArgumentNullException(nameof(fuelTypeRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }
        [Produces("application/json",
        "application/vnd.marvin.hateoas+json",
        "application/vnd.marvin.getfueltypes.full+json",
        "application/vnd.marvin.getfueltypes.full.hateoas+json",
        "application/vnd.marvin.getfueltypes.friendly+json",
        "application/vnd.marvin.getfueltypes.friendly.hateoas+json")]
        [HttpGet(Name = "GetFuelTypes")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        public IActionResult GetFuelTypes(
        [FromQuery] FuelTypeResourceParameters fuelTypeResourceParameters)
        {
            if (!_propertyMappingService.ValidFuelTypeMappingExistsFor<FuelTypeDto, Entities.FuelType>
               (fuelTypeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var fuelTypeFromRepo = _fuelTypeRepository.GetFuelTypes(fuelTypeResourceParameters);

            var paginationMetadata = new
            {
                totalCount = fuelTypeFromRepo.TotalCount,
                pageSize = fuelTypeFromRepo.PageSize,
                currentPage = fuelTypeFromRepo.CurrentPage,
                totalPages = fuelTypeFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForFuelType(fuelTypeResourceParameters,
                fuelTypeFromRepo.HasNext,
                fuelTypeFromRepo.HasPrevious);

            var shapedFuelTypes = _mapper.Map<IEnumerable<FuelTypeDto>>(fuelTypeFromRepo)
                               .ShapeData(fuelTypeResourceParameters.Fields);

            var shapedFuelTypesWithLinks = shapedFuelTypes.Select(fueltypes =>
            {
                var fuelTypeAsDictionary = fueltypes as IDictionary<string, object>;
                var fuelTypeLinks = CreateLinksForFuelType((string)fuelTypeAsDictionary["Id"], null);
                fuelTypeAsDictionary.Add("links", fuelTypeLinks);
                return fuelTypeAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedFuelTypesWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [Produces("application/json",
          "application/vnd.marvin.hateoas+json",
          "application/vnd.marvin.getfueltype.full+json",
          "application/vnd.marvin.getfueltype.full.hateoas+json",
          "application/vnd.marvin.getfueltype.friendly+json",
          "application/vnd.marvin.getfueltype.friendly.hateoas+json")]
        [HttpGet("{fueltypeId}", Name = "GetFuelType")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.getfueltype+json")]
        [Consumes("application/json",
            "application/vnd.marvin.getfueltype+json")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetFuelType(Guid fuelTypeId, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<FuelTypeDto>
               (fields))
            {
                return BadRequest();
            }

            var fuelTypeFromRepo = _fuelTypeRepository.GetFuelType(fuelTypeId);

            if (fuelTypeFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForFuelType(fuelTypeId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full Site
            if (primaryMediaType == "vnd.marvin.fueltype.full")
            {
                var fullResourceToReturn = _mapper.Map<FuelTypeFullDto>(fuelTypeFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly site
            var friendlyResourceToReturn = _mapper.Map<FuelTypeDto>(fuelTypeFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

        [HttpPost(Name = "CreateFuelType")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.createfueltype+json")]
        [Consumes("application/json",
            "application/vnd.marvin.createfueltype+json")]
        public ActionResult<FuelTypeDto> CreateFuelType(FuelTypeForCreationDto fuelType)
        {

            var fuelTypeEntity = _mapper.Map<Entities.FuelType>(fuelType);

            _fuelTypeRepository.AddFuelType(fuelTypeEntity);
            _fuelTypeRepository.Save();

            var fuelTypeToReturn = _mapper.Map<FuelTypeDto>(fuelTypeEntity);

            var links = CreateLinksForFuelType(fuelTypeToReturn.Id.ToString(), null);

            var linkedResourceToReturn = fuelTypeToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetFuelType",
                new { FuelTypeId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.fueltypeforpatch+json")]
        [Consumes("application/json",
           "application/vnd.marvin.fueltypeforpatch+json")]
        [HttpPatch("{fuelTypeId}", Name = "PatchFuelType" )]
        public ActionResult PatchFuelType(Guid fuelTypeId, [FromBody]JsonPatchDocument<FuelTypeForUpdateDto> patchDocument)
        {

            if (fuelTypeId == null)
            {
                return BadRequest();
            }

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_fuelTypeRepository.FuelTypeExists(fuelTypeId))
            {
                return NotFound();
            }

            var fuelTypeFromRepo = _fuelTypeRepository.GetFuelType(fuelTypeId);

            if (fuelTypeFromRepo == null)
            {
                var FuelTypeDto = new FuelTypeForUpdateDto();
                patchDocument.ApplyTo(FuelTypeDto, ModelState);

                if (!TryValidateModel(FuelTypeDto))
                {
                    return ValidationProblem(ModelState);
                }

                var fuelTypeToAdd = _mapper.Map<Entities.FuelType>(fuelTypeId);

                _fuelTypeRepository.AddFuelType(fuelTypeId, fuelTypeToAdd);

                _fuelTypeRepository.Save();

                var fuelTypeToReturn = _mapper.Map<FuelTypeDto>(fuelTypeToAdd);

                return CreatedAtRoute("GetFuelType",
                    new { fuelTypeId }, fuelTypeToReturn);
            }

            var fuelTypeToPatch = _mapper.Map<FuelTypeForUpdateDto>(fuelTypeFromRepo);

            patchDocument.ApplyTo(fuelTypeToPatch);

            if (!TryValidateModel(fuelTypeToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(fuelTypeToPatch, fuelTypeFromRepo);

            _fuelTypeRepository.UpdateFuelType(fuelTypeFromRepo);

            _fuelTypeRepository.Save();

            return NoContent();
        }

        [HttpDelete("{fuelTypeId}", Name = "DeleteFuelType")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.deletefuelType+json")]
        [Consumes("application/json",
            "application/vnd.marvin.deletefuelType+json")]
        public ActionResult DeleteFuelType(Guid fuelTypeId)
        {
            var fuelTypeFromRepo = _fuelTypeRepository.GetFuelType(fuelTypeId);

            if (fuelTypeFromRepo == null)
            {
                return NotFound();
            }

            _fuelTypeRepository.DeleteFuelType(fuelTypeFromRepo);

            _fuelTypeRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetSiteOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
        private string CreateFuelTypesResourceUri(
          FuelTypeResourceParameters fuelTypeResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetFuelType",
                      new
                      {
                          fields = fuelTypeResourceParameters.Fields,
                          orderBy = fuelTypeResourceParameters.OrderBy,
                          pageNumber = fuelTypeResourceParameters.PageNumber - 1,
                          pageSize = fuelTypeResourceParameters.PageSize,
                          searchQuery = fuelTypeResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetFuelType",
                      new
                      {
                          fields = fuelTypeResourceParameters.Fields,
                          orderBy = fuelTypeResourceParameters.OrderBy,
                          pageNumber = fuelTypeResourceParameters.PageNumber + 1,
                          pageSize = fuelTypeResourceParameters.PageSize,
                          searchQuery = fuelTypeResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetFuelType",
                    new
                    {
                        fields = fuelTypeResourceParameters.Fields,
                        orderBy = fuelTypeResourceParameters.OrderBy,
                        pageNumber = fuelTypeResourceParameters.PageNumber,
                        pageSize = fuelTypeResourceParameters.PageSize,
                        searchQuery = fuelTypeResourceParameters.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForFuelType(string fuelTypeId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetFuelType", new { fuelTypeId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetFuelType", new { fuelTypeId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("PatchFuelType", new { fuelTypeId }),
                   "patch_fueltype",
                   "PATCH"));

                links.Add(
                   new LinkDto(Url.Link("DeleteFuelType", new { fuelTypeId }),
                   "delete_fueltype",
                   "DELETE"));

            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForFuelType(
        FuelTypeResourceParameters fuelTypeResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateFuelTypesResourceUri(
                   fuelTypeResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateFuelTypesResourceUri(
                      fuelTypeResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateFuelTypesResourceUri(
                        fuelTypeResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}