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
using Siteminder.API.ActionConstraints;
using Microsoft.AspNetCore.JsonPatch;

namespace Siteminder.API.Controllers
{
    [ApiController]
    [Route("api/sites/{siteid}")]
    public class TerminalController : ControllerBase
    {
        private readonly ITerminalRepository _terminalRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public TerminalController(ITerminalRepository terminalRepository,
            IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _terminalRepository = terminalRepository ??
              throw new ArgumentNullException(nameof(terminalRepository));
            _mapper = mapper ??
              throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }
        [Produces("application/json",
         "application/vnd.marvin.hateoas+json",
         "application/vnd.marvin.siteterminals.full+json",
         "application/vnd.marvin.siteterminals.full.hateoas+json",
         "application/vnd.marvin.siteterminals.friendly+json",
         "application/vnd.marvin.siteterminals.friendly.hateoas+json")]
        [HttpGet("terminals",Name = "GetTerminals")]
        [HttpHead]
        public IActionResult GetTerminals(Guid siteId,
      [FromQuery] TerminalResourceParameters terminalResourceParameters)
        {
            if (siteId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(siteId));
            }

            if (!_propertyMappingService.ValidTerminalMappingExistsFor<TerminalDto, Entities.Terminal>
               (terminalResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            terminalResourceParameters.SiteId = siteId;

            var terminalFromRepo = _terminalRepository.GetTerminals(terminalResourceParameters);

            var paginationMetadata = new
            {
                totalCount = terminalFromRepo.TotalCount,
                pageSize = terminalFromRepo.PageSize,
                currentPage = terminalFromRepo.CurrentPage,
                totalPages = terminalFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForTerminal(terminalResourceParameters,
                terminalFromRepo.HasNext,
                terminalFromRepo.HasPrevious);

            var shapedTerminals = _mapper.Map<IEnumerable<TerminalDto>>(terminalFromRepo)
                               .ShapeData(terminalResourceParameters.Fields);

            var shapedTerminalWithLinks = shapedTerminals.Select(terminal =>
            {
                var terminalAsDictionary = terminal as IDictionary<string, object>;
                var terminalLinks = CreateLinksForTerminal(siteId.ToString(), (string)terminalAsDictionary["Id"], null);
                terminalAsDictionary.Add("links", terminalLinks);
                return terminalAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedTerminalWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [Produces("application/json",
          "application/vnd.marvin.hateoas+json",
          "application/vnd.marvin.siteterminal.full+json",
          "application/vnd.marvin.siteterminal.full.hateoas+json",
          "application/vnd.marvin.siteterminal.friendly+json",
          "application/vnd.marvin.siteterminal.friendly.hateoas+json")]
        [HttpGet("terminals/{terminalId}", Name = "GetTerminal")]
        public IActionResult GetTerminal(Guid siteId, Guid terminalId, string fields,
          [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<TerminalDto>
               (fields))
            {
                return BadRequest();
            }

            var terminalFromRepo = _terminalRepository.GetTerminal(siteId, terminalId);

            if (terminalFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForTerminal(siteId.ToString(), terminalId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full site
            if (primaryMediaType == "vnd.marvin.terminal.full")
            {
                var fullResourceToReturn = _mapper.Map<TerminalFullDto>(terminalFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly terminal
            var friendlyResourceToReturn = _mapper.Map<TerminalDto>(terminalFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

        [HttpPost("terminals", Name = "CreateTerminal")]
        [RequestHeaderMatchesMediaType("Content-Type",
           "application/json",
           "application/vnd.marvin.terminalforcreation+json")]
        [Consumes("application/json",
           "application/vnd.marvin.terminalforcreation+json")]
        public ActionResult<TerminalDto> CreateTerminal(Guid siteId, TerminalForCreationDto terminal)
        {
            var terminalEntity = _mapper.Map<Entities.Terminal>(terminal);
            terminalEntity.SiteId = siteId;
            _terminalRepository.AddTerminal(terminalEntity);
            _terminalRepository.Save();

            var terminalToReturn = _mapper.Map<TerminalDto>(terminalEntity);

            var links = CreateLinksForTerminal(siteId.ToString(), terminalToReturn.Id, null);

            var linkedResourceToReturn = terminalToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetTerminal",
                new { siteId, terminalId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [RequestHeaderMatchesMediaType("Content-Type",
             "application/json",
             "application/vnd.marvin.terminalforpatch+json")]
        [Consumes("application/json",
             "application/vnd.marvin.terminalforpatch+json")]
        [HttpPatch("terminals/{terminalId}", Name ="PatchTerminal")]
        public ActionResult PatchTerminal(Guid siteId, Guid terminalId, [FromBody]JsonPatchDocument<TerminalForUpdateDto> patchDocument)
        {

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_terminalRepository.TerminalExists(terminalId))
            {
                return NotFound();
            }

            var terminalFromRepo = _terminalRepository.GetTerminal(siteId, terminalId);

            if (terminalFromRepo == null)
            {
                var terminalDto = new TerminalForUpdateDto();
                patchDocument.ApplyTo(terminalDto, ModelState);

                if (!TryValidateModel(terminalDto))
                {
                    return ValidationProblem(ModelState);
                }

                var terminalToAdd = _mapper.Map<Entities.Terminal>(terminalDto);
                terminalToAdd.Id = terminalId;
                terminalToAdd.SiteId = siteId;

                _terminalRepository.AddTerminal(terminalToAdd);

                _terminalRepository.Save();

                var terminalToReturn = _mapper.Map<TerminalDto>(terminalToAdd);

                return CreatedAtRoute("GetTerminal",
                    new { terminalId }, terminalToReturn);
            }

            var terminalToPatch = _mapper.Map<TerminalForUpdateDto>(terminalFromRepo);

            patchDocument.ApplyTo(terminalToPatch);

            if (!TryValidateModel(terminalToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(terminalToPatch, terminalFromRepo);

            _terminalRepository.UpdateTerminal(terminalId, terminalFromRepo);

            _terminalRepository.Save();

            return NoContent();

        }

        [HttpDelete("terminals/{terminalId}", Name = "DeleteTerminal")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.deleteterminal+json")]
        [Consumes("application/json",
            "application/vnd.marvin.deleteterminal+json")]
        public ActionResult DeleteTerminal(Guid siteId,Guid terminalId)
        {
            var terminalFromRepo = _terminalRepository.GetTerminal(siteId, terminalId);

            if (terminalFromRepo == null)
            {
                return NotFound();
            }

            _terminalRepository.DeleteTerminal(terminalFromRepo);

            _terminalRepository.Save();

            return NoContent();
        }
       
        [HttpOptions]
        public IActionResult GetTerminalOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }
        private string CreateTerminalResourceUri(
          TerminalResourceParameters terminalResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetTerminal",
                      new
                      {
                          fields = terminalResourceParameters.Fields,
                          orderBy = terminalResourceParameters.OrderBy,
                          pageNumber = terminalResourceParameters.PageNumber - 1,
                          pageSize = terminalResourceParameters.PageSize,
                          searchQuery = terminalResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetTerminal",
                      new
                      {
                          fields = terminalResourceParameters.Fields,
                          orderBy = terminalResourceParameters.OrderBy,
                          pageNumber = terminalResourceParameters.PageNumber + 1,
                          pageSize = terminalResourceParameters.PageSize,
                          searchQuery = terminalResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetTerminal",
                    new
                    {
                        fields = terminalResourceParameters.Fields,
                        orderBy = terminalResourceParameters.OrderBy,
                        pageNumber = terminalResourceParameters.PageNumber,
                        pageSize = terminalResourceParameters.PageSize,
                        searchQuery = terminalResourceParameters.SearchQuery
                    });
            }

        }
        private IEnumerable<LinkDto> CreateLinksForTerminal(string siteId, string terminalId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetTerminal", new { siteId, terminalId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetTerminal", new { siteId, terminalId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                    new LinkDto(Url.Link("CreateTerminal", new { siteId, terminalId }),
                    "create_terminal",
                    "POST"));
                
                links.Add(
                   new LinkDto(Url.Link("PatchTerminal", new { siteId, terminalId }),
                   "patch_terminal",
                   "PATCH"));

                links.Add(
                   new LinkDto(Url.Link("DeleteTerminal", new { siteId, terminalId }),
                   "delete_terminal",
                   "DELETE"));

            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForTerminal(
        TerminalResourceParameters terminalResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateTerminalResourceUri(
                   terminalResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateTerminalResourceUri(
                      terminalResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateTerminalResourceUri(
                        terminalResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
