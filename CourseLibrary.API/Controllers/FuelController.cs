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
    [Route("api/fueltypes/{fueltypeId}/fuel")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class FuelController : ControllerBase
    {
        private readonly IFuelRepository _fuelRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public FuelController(IFuelRepository fuelRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _fuelRepository = fuelRepository ??
            throw new ArgumentNullException(nameof(fuelRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }
        [Produces("application/json",
            "application/vnd.marvin.hateoas+json",
            "application/vnd.marvin.getfuelsbytype.full+json",
            "application/vnd.marvin.getfuelsbytype.full.hateoas+json",
            "application/vnd.marvin.getfuelsbytype.friendly+json",
            "application/vnd.marvin.getfuelsbytype.friendly.hateoas+json")]
        [HttpGet(Name = "GetFuelsByType")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        public IActionResult GetFuelsByType(Guid fuelTypeId,
            [FromQuery] FuelResourceParameters fuelResourceParameters)
        {
            if (!_propertyMappingService.ValidFuelMappingExistsFor<FuelDto, Entities.Fuel>
               (fuelResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var fuelFromRepo = _fuelRepository.GetFuelByType(fuelTypeId, fuelResourceParameters);

            var paginationMetadata = new
            {
                totalCount = fuelFromRepo.TotalCount,
                pageSize = fuelFromRepo.PageSize,
                currentPage = fuelFromRepo.CurrentPage,
                totalPages = fuelFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForFuel(fuelResourceParameters,
                fuelFromRepo.HasNext,
                fuelFromRepo.HasPrevious);

            var shapedFuel = _mapper.Map<IEnumerable<FuelDto>>(fuelFromRepo)
                               .ShapeData(fuelResourceParameters.Fields);

            var shapedFuelTypesWithLinks = shapedFuel.Select(fuel =>
            {
                var fuelAsDictionary = fuel as IDictionary<string, object>;
                var fuelLinks = CreateLinksForFuel(fuelTypeId.ToString(),(string)fuelAsDictionary["Id"], null);
                fuelAsDictionary.Add("links", fuelLinks);
                return fuelAsDictionary;
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
          "application/vnd.marvin.getfuel.full+json",
          "application/vnd.marvin.getfuel.full.hateoas+json",
          "application/vnd.marvin.getfuel.friendly+json",
          "application/vnd.marvin.getfuel.friendly.hateoas+json")]
        [HttpGet("{fuelId}", Name = "GetFuel")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.getfuel+json")]
        [Consumes("application/json",
            "application/vnd.marvin.getfuel+json")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetFuel(Guid fuelTypeId, Guid fuelId, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<FuelDto>
               (fields))
            {
                return BadRequest();
            }

            var fuelFromRepo = _fuelRepository.GetFuel(fuelId);

            if (fuelFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForFuel(fuelTypeId.ToString(), fuelId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full Site
            if (primaryMediaType == "vnd.marvin.fueltype.full")
            {
                var fullResourceToReturn = _mapper.Map<FuelFullDto>(fuelFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly site
            var friendlyResourceToReturn = _mapper.Map<FuelDto>(fuelFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

        [HttpPost(Name = "CreateFuel")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.createfuel+json")]
        [Consumes("application/json",
            "application/vnd.marvin.createfuel+json")]
        public ActionResult<FuelDto> CreateFuel(Guid fuelTypeId, FuelForCreationDto fuel)
        {

            var fuelEntity = _mapper.Map<Entities.Fuel>(fuel);

            fuelEntity.FuelTypeId = fuelTypeId;

            _fuelRepository.AddFuel(fuelEntity);
            _fuelRepository.Save();

            var fuelToReturn = _mapper.Map<FuelDto>(fuelEntity);

            var links = CreateLinksForFuel(fuelToReturn.FuelTypeId.ToString(), fuelToReturn.Id.ToString(), null);

            var linkedResourceToReturn = fuelToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetFuel",
                new { FuelTypeId = fuelToReturn.FuelTypeId.ToString(), FuelId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.fuelforpatch+json")]
        [Consumes("application/json",
           "application/vnd.marvin.fuelforpatch+json")]
        [HttpPatch("{fuelId}", Name = "PatchFuel")]
        public ActionResult PatchFuel(Guid fuelTypeId, Guid fuelId, [FromBody]JsonPatchDocument<FuelForUpdateDto> patchDocument)
        {

            if (fuelId == null)
            {
                return BadRequest();
            }

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_fuelRepository.FuelExists(fuelId))
            {
                return NotFound();
            }

            var fuelFromRepo = _fuelRepository.GetFuel(fuelId);

            if (fuelFromRepo == null)
            {
                var FuelDto = new FuelForUpdateDto();
                patchDocument.ApplyTo(FuelDto, ModelState);

                if (!TryValidateModel(FuelDto))
                {
                    return ValidationProblem(ModelState);
                }

                var fuelToAdd = _mapper.Map<Entities.Fuel>(fuelId);

                _fuelRepository.AddFuel(fuelTypeId, fuelId, fuelToAdd);

                _fuelRepository.Save();

                var fuelTypeToReturn = _mapper.Map<FuelDto>(fuelToAdd);

                return CreatedAtRoute("GetFuel",
                    new {fuelTypeId,  fuelId }, fuelTypeToReturn);
            }

            var fuelToPatch = _mapper.Map<FuelForUpdateDto>(fuelFromRepo);

            patchDocument.ApplyTo(fuelToPatch);

            if (!TryValidateModel(fuelToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(fuelToPatch, fuelFromRepo);

            _fuelRepository.UpdateFuel(fuelFromRepo);

            _fuelRepository.Save();

            return NoContent();
        }

        [HttpDelete("{fuelId}", Name = "DeleteFuel")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.deletefuel+json")]
        [Consumes("application/json",
            "application/vnd.marvin.deletefuel+json")]
        public ActionResult DeleteFuel(Guid fuelId)
        {
            var fuelFromRepo = _fuelRepository.GetFuel(fuelId);

            if (fuelFromRepo == null)
            {
                return NotFound();
            }

            _fuelRepository.DeleteFuel(fuelFromRepo);

            _fuelRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetFuelOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PATCH");
            return Ok();
        }
        private string CreateFuelResourceUri(
          FuelResourceParameters fuelResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetFuel",
                      new
                      {
                          fields = fuelResourceParameters.Fields,
                          orderBy = fuelResourceParameters.OrderBy,
                          pageNumber = fuelResourceParameters.PageNumber - 1,
                          pageSize = fuelResourceParameters.PageSize,
                          searchQuery = fuelResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetFuel",
                      new
                      {
                          fields = fuelResourceParameters.Fields,
                          orderBy = fuelResourceParameters.OrderBy,
                          pageNumber = fuelResourceParameters.PageNumber + 1,
                          pageSize = fuelResourceParameters.PageSize,
                          searchQuery = fuelResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetFuel",
                    new
                    {
                        fields = fuelResourceParameters.Fields,
                        orderBy = fuelResourceParameters.OrderBy,
                        pageNumber = fuelResourceParameters.PageNumber,
                        pageSize = fuelResourceParameters.PageSize,
                        searchQuery = fuelResourceParameters.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForFuel(string fuelTypeId, string fuelId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetFuel", new { fuelTypeId, fuelId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetFuel", new { fuelTypeId, fuelId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("PatchFuel", new { fuelTypeId, fuelId }),
                   "patch_fuel",
                   "PATCH"));

                links.Add(
                   new LinkDto(Url.Link("DeleteFuel", new { fuelTypeId, fuelId }),
                   "delete_fuel",
                   "DELETE"));

            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForFuel(
        FuelResourceParameters fuelResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateFuelResourceUri(
                   fuelResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateFuelResourceUri(
                      fuelResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateFuelResourceUri(
                        fuelResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
