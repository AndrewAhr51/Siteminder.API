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
    [Route("api/terminals/{terminalId}/dispensers")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class DispenserController : ControllerBase
    {
        private readonly IDispenserRepository _dispenserRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public DispenserController(IDispenserRepository dispenserRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _dispenserRepository = dispenserRepository ??
               throw new ArgumentNullException(nameof(dispenserRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }
        [Produces("application/json",
           "application/vnd.marvin.hateoas+json",
           "application/vnd.marvin.getdispensers.full+json",
           "application/vnd.marvin.getdispensers.full.hateoas+json",
           "application/vnd.marvin.getdispensers.friendly+json",
           "application/vnd.marvin.getdispensers.friendly.hateoas+json")]
        [HttpGet(Name = "GetDispensers")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        public IActionResult GetDispensers(Guid terminalId,
            [FromQuery] DispenserResourceParameters dispenserResourceParameters)
        {
            if (!_propertyMappingService.ValidCompanyMappingExistsFor<CompanyDto, Entities.Company>
               (dispenserResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            dispenserResourceParameters.TerminalId = terminalId;
            var dispenserFromRepo = _dispenserRepository.GetDispensers(dispenserResourceParameters);

            var paginationMetadata = new
            {
                totalCount = dispenserFromRepo.TotalCount,
                pageSize = dispenserFromRepo.PageSize,
                currentPage = dispenserFromRepo.CurrentPage,
                totalPages = dispenserFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForDispenser(dispenserResourceParameters,
                dispenserFromRepo.HasNext,
                dispenserFromRepo.HasPrevious);

            var shapedDispensers = _mapper.Map<IEnumerable<DispenserDto>>(dispenserFromRepo)
                               .ShapeData(dispenserResourceParameters.Fields);

            var shapedDispensersWithLinks = shapedDispensers.Select(dispenser =>
            {
                var dispenserAsDictionary = dispenser as IDictionary<string, object>;
                var dispenserLinks = CreateLinksForDispenser(terminalId.ToString(), dispenserAsDictionary["Id"].ToString(), null);
                dispenserAsDictionary.Add("links", dispenserLinks);
                return dispenserAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedDispensersWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [Produces("application/json",
            "application/vnd.marvin.hateoas+json",
            "application/vnd.marvin.getdispenser.full+json",
            "application/vnd.marvin.getdispenser.full.hateoas+json",
            "application/vnd.marvin.getdispenser.friendly+json",
            "application/vnd.marvin.getdispenser.friendly.hateoas+json")]
        [HttpGet("{dispenserId}", Name = "GetDispenser")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetDispenser(Guid terminalId, Guid dispenserId, string fields,
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

            var dispenserFromRepo = _dispenserRepository.GetDispenser(dispenserId);

            if (dispenserFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForDispenser(terminalId.ToString(), dispenserId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full author
            if (primaryMediaType == "vnd.marvin.dispenser.full")
            {
                var fullResourceToReturn = _mapper.Map<CompanyFullDto>(dispenserFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly author
            var friendlyResourceToReturn = _mapper.Map<DispenserDto>(dispenserFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

        [HttpPost (Name = "CreateDispenser")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.dispenserforcreation+json")]
        [Consumes("application/json",
            "application/vnd.marvin.dispenserforcreation+json")]
        public ActionResult<DispenserDto> CreateDispenser(Guid terminalId, DispenserForCreationDto dispenser)
        {
            var dispenserEntity = _mapper.Map<Entities.Dispenser>(dispenser);

            dispenserEntity.TerminalId = terminalId;

            _dispenserRepository.AddDispenser(dispenserEntity);
            _dispenserRepository.Save();

            var dispenserToReturn = _mapper.Map<DispenserDto>(dispenserEntity);

            var links = CreateLinksForDispenser(terminalId.ToString(),dispenserToReturn.Id.ToString(), null);

            var linkedResourceToReturn = dispenserToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetDispenser",
                new { terminalId, dispenserId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.dispenserforpatch+json")]
        [Consumes("application/json",
            "application/vnd.marvin.dispenserforpatch+json")]
        [HttpPatch("{dispenserId}", Name = "PatchDispenser")]
        public ActionResult PatchDispenser(Guid dispenserId, [FromBody]JsonPatchDocument<DispenserForUpdateDto> patchDocument)
        {

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_dispenserRepository.DispenserExists(dispenserId))
            {
                return NotFound();
            }

            var dispenserFromRepo = _dispenserRepository.GetDispenser(dispenserId);

            if (dispenserFromRepo == null)
            {
                var dispenserDto = new DispenserForUpdateDto();
                patchDocument.ApplyTo(dispenserDto, ModelState);

                if (!TryValidateModel(dispenserDto))
                {
                    return ValidationProblem(ModelState);
                }

                var dispenserToAdd = _mapper.Map<Entities.Dispenser>(dispenserDto);
                dispenserToAdd.Id = dispenserId;

                _dispenserRepository.AddDispenser(dispenserId, dispenserToAdd);

                _dispenserRepository.Save();

                var dispenserToReturn = _mapper.Map<DispenserDto>(dispenserToAdd);

                return CreatedAtRoute("GetDispenser",
                    new { dispenserId }, dispenserToReturn);
            }

            var dispenserToPatch = _mapper.Map<DispenserForUpdateDto>(dispenserFromRepo);

            patchDocument.ApplyTo(dispenserToPatch, ModelState);

            if (!TryValidateModel(dispenserToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(dispenserToPatch, dispenserFromRepo);

            _dispenserRepository.UpdateDispenser(dispenserFromRepo);

            _dispenserRepository.Save();

            return NoContent();

        }

        [HttpDelete("{dispenserId}", Name = "DeleteDispenser")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.dispenserfordeletion+json")]
        [Consumes("application/json",
            "application/vnd.marvin.dispenserfordeletion+json")]
        public ActionResult DeleteDispenser(Guid dispenserId)
        {
            var dispenserFromRepo = _dispenserRepository.GetDispenser(dispenserId);

            if (dispenserFromRepo == null)
            {
                return NotFound();
            }

            _dispenserRepository.DeleteDispenser(dispenserFromRepo);

            _dispenserRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetDispenserOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PATCH");
            return Ok();
        }
        private string CreateDispensersResourceUri(
          DispenserResourceParameters dispenserResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetDispensers",
                      new
                      {
                          fields = dispenserResourceParameters.Fields,
                          orderBy = dispenserResourceParameters.OrderBy,
                          pageNumber = dispenserResourceParameters.PageNumber - 1,
                          pageSize = dispenserResourceParameters.PageSize,
                          mainCategory = dispenserResourceParameters.Type,
                          searchQuery = dispenserResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetDispensers",
                      new
                      {
                          fields = dispenserResourceParameters.Fields,
                          orderBy = dispenserResourceParameters.OrderBy,
                          pageNumber = dispenserResourceParameters.PageNumber + 1,
                          pageSize = dispenserResourceParameters.PageSize,
                          mainCategory = dispenserResourceParameters.Type,
                          searchQuery = dispenserResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetDispensers",
                    new
                    {
                        fields = dispenserResourceParameters.Fields,
                        orderBy = dispenserResourceParameters.OrderBy,
                        pageNumber = dispenserResourceParameters.PageNumber,
                        pageSize = dispenserResourceParameters.PageSize,
                        mainCategory = dispenserResourceParameters.Type,
                        searchQuery = dispenserResourceParameters.SearchQuery
                    });
            }

        }

        private IEnumerable<LinkDto> CreateLinksForDispenser(string terminalId, string dispenserId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetDispenser", new { terminalId, dispenserId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetDispenser", new { terminalId, dispenserId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("DeleteDispenser", new { terminalId, dispenserId }),
                   "delete_dispenser",
                   "DELETE"));

                links.Add(
                   new LinkDto(Url.Link("PatchDispenser", new { terminalId, dispenserId }),
                   "patch_dispenser",
                   "PATCH"));
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForDispenser(
        DispenserResourceParameters dispenserResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateDispensersResourceUri(
                   dispenserResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateDispensersResourceUri(
                      dispenserResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateDispensersResourceUri(
                        dispenserResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
