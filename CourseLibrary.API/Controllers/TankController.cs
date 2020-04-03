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
using Siteminder.API.Services.Tanks;
using Siteminder.API.Models.Tanks;

namespace Siteminder.API.Controllers
{
    [ApiController]
    [Route("api/terminals/{terminalId}/dispensers/{dispenserId}/fueltypes/{fuelTypeId}")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class TankController : ControllerBase
    {
        private readonly ITankRepository _tankRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public TankController(ITankRepository tankRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _tankRepository = tankRepository ??
               throw new ArgumentNullException(nameof(tankRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [Produces("application/json",
         "application/vnd.marvin.hateoas+json",
         "application/vnd.marvin.gettanks.full+json",
         "application/vnd.marvin.gettanks.full.hateoas+json",
         "application/vnd.marvin.gettanks.friendly+json",
         "application/vnd.marvin.gettanks.friendly.hateoas+json")]
        [HttpGet("sites/sites", Name = "GetTanks")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        public IActionResult GetTanks(Guid terminalId, Guid dispenserId, Guid fuelTypeId,
          [FromQuery] TankResourceParameters tankResourceParameters)
        {
            if (!_propertyMappingService.ValidTankMappingExistsFor<TankDto, Entities.Tank>
               (tankResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            tankResourceParameters.TermimalId = terminalId;
            tankResourceParameters.DispenserId = dispenserId;
            tankResourceParameters.FuelTypeId = fuelTypeId;

            var tanksFromRepo = _tankRepository.GetTanks(tankResourceParameters);

            var paginationMetadata = new
            {
                totalCount = tanksFromRepo.TotalCount,
                pageSize = tanksFromRepo.PageSize,
                currentPage = tanksFromRepo.CurrentPage,
                totalPages = tanksFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForTank(tankResourceParameters,
                tanksFromRepo.HasNext,
                tanksFromRepo.HasPrevious);

            var shapedTank = _mapper.Map<IEnumerable<TankDto>>(tanksFromRepo)
                               .ShapeData(tankResourceParameters.Fields);

            var shapedTankWithLinks = shapedTank.Select(tanks =>
            {
                var tankAsDictionary = tanks as IDictionary<string, object>;
                var tankLinks = CreateLinksForTank(terminalId.ToString(), dispenserId.ToString(),fuelTypeId.ToString(), (string)tankAsDictionary["Id"], null);
                tankAsDictionary.Add("links", tankLinks);
                return tankAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedTankWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [Produces("application/json",
        "application/vnd.marvin.hateoas+json",
        "application/vnd.marvin.gettank.full+json",
        "application/vnd.marvin.gettank.full.hateoas+json",
        "application/vnd.marvin.gettank.friendly+json",
        "application/vnd.marvin.gettank.friendly.hateoas+json")]
        [HttpGet("tanks/{tankId}", Name = "GetTank")]
        [RequestHeaderMatchesMediaType("Content-Type",
          "application/json",
          "application/vnd.marvin.gettank+json")]
        [Consumes("application/json",
          "application/vnd.marvin.gettank+json")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetTank(Guid terminalId, Guid dispenserId, Guid fuelTypeId, Guid tankId, string fields,
          [FromHeader(Name = "Accept")] string mediaType)
        {


            TankResourceParameters tankResourceParameters = new TankResourceParameters();

            tankResourceParameters.TermimalId = terminalId;
            tankResourceParameters.DispenserId = dispenserId;
            tankResourceParameters.TermimalId = fuelTypeId;

            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<TankDto>
               (fields))
            {
                return BadRequest();
            }

            var tankFromRepo = _tankRepository.GetTank(tankResourceParameters, tankId);

            if (tankFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForTank(terminalId.ToString(), dispenserId.ToString(), fuelTypeId.ToString(),tankId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full tank
            if (primaryMediaType == "vnd.marvin.tank.full")
            {
                var fullResourceToReturn = _mapper.Map<TankFullDto>(tankFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly tank
            var friendlyResourceToReturn = _mapper.Map<TankDto>(tankFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }


        [HttpPost("tanks", Name = "CreateTank")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.createtank+json")]
        [Consumes("application/json",
            "application/vnd.marvin.createtank+json")]
        public ActionResult<TankDto> CreateTank(Guid terminalId, Guid dispenserId, Guid fuelTypeId, TankForCreationDto tank)
        {

            var tankEntity = _mapper.Map<Entities.Tank>(tank);
            tankEntity.TerminalId = terminalId;
            tankEntity.DispenserId = dispenserId;
            tankEntity.FuelTypeId = fuelTypeId;

            _tankRepository.AddTank(tankEntity);
            _tankRepository.Save();

            var siteToReturn = _mapper.Map<TankDto>(tankEntity);

            var links = CreateLinksForTank(terminalId.ToString(), dispenserId.ToString(), fuelTypeId.ToString(), null, null);

            var linkedResourceToReturn = siteToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetTank",
                new { terminalId, dispenserId, fuelTypeId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [RequestHeaderMatchesMediaType("Content-Type",
        "application/json",
        "application/vnd.marvin.tankforpatch+json")]
        [Consumes("application/json",
       "application/vnd.marvin.tankforpatch+json")]
        [HttpPatch("sites/{siteId}", Name = "PatchTank")]
        public ActionResult PatchSite(Guid terminalId, Guid dispenserId, Guid fuelTypeId, Guid tankId, [FromBody]JsonPatchDocument<TankForUpdateDto> patchDocument)
        {

            TankResourceParameters tankResourceParameters = new TankResourceParameters();
            if (terminalId == Guid.Empty || dispenserId == Guid.Empty ||  fuelTypeId  == Guid.Empty || tankId == Guid.Empty)
            {
                return BadRequest();
            }

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_tankRepository.TankExists(tankId))
            {
                return NotFound();
            }

            tankResourceParameters.TermimalId = terminalId;
            tankResourceParameters.DispenserId = dispenserId;
            tankResourceParameters.FuelTypeId = fuelTypeId;

            var siteFromRepo = _tankRepository.GetTank(tankResourceParameters, tankId);

            if (siteFromRepo == null)
            {
                var tankDto = new TankForUpdateDto();
                patchDocument.ApplyTo(tankDto, ModelState);

                if (!TryValidateModel(tankDto))
                {
                    return ValidationProblem(ModelState);
                }

                var tankToAdd = _mapper.Map<Entities.Tank>(tankDto);
                tankToAdd.TerminalId = terminalId;
                tankToAdd.DispenserId = dispenserId;
                tankToAdd.FuelTypeId = fuelTypeId;
                

                _tankRepository.AddTank(tankToAdd, tankId);

                _tankRepository.Save();

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


        [HttpOptions]
        public IActionResult GetTankOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PATCH");
            return Ok();
        }
        private string CreateTanksResourceUri(
          TankResourceParameters tankResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetTanks",
                      new
                      {
                          fields = tankResourceParameters.Fields,
                          orderBy = tankResourceParameters.OrderBy,
                          pageNumber = tankResourceParameters.PageNumber - 1,
                          pageSize = tankResourceParameters.PageSize,
                          searchQuery = tankResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetTanks",
                      new
                      {
                          fields = tankResourceParameters.Fields,
                          orderBy = tankResourceParameters.OrderBy,
                          pageNumber = tankResourceParameters.PageNumber + 1,
                          pageSize = tankResourceParameters.PageSize,
                          searchQuery = tankResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetTanks",
                    new
                    {
                        fields = tankResourceParameters.Fields,
                        orderBy = tankResourceParameters.OrderBy,
                        pageNumber = tankResourceParameters.PageNumber,
                        pageSize = tankResourceParameters.PageSize,
                        searchQuery = tankResourceParameters.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForTank(string terminalId, string dispenserId, string fuelTypeId, string tankId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetTank", new { terminalId, dispenserId, fuelTypeId, tankId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetTank", new { terminalId, dispenserId, fuelTypeId, tankId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("PatchTank", new { terminalId, dispenserId, fuelTypeId, tankId }),
                   "patch_tank",
                   "PATCH"));

                links.Add(
                   new LinkDto(Url.Link("DeleteTank", new { terminalId, dispenserId, fuelTypeId, tankId }),
                   "delete_tank",
                   "DELETE"));

            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForTank(
        TankResourceParameters tankResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateTanksResourceUri(
                   tankResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateTanksResourceUri(
                      tankResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateTanksResourceUri(
                        tankResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
