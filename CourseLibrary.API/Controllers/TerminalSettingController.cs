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
using Marvin.Cache.Headers;

namespace Siteminder.API.Controllers
{
    [ApiController]
    [Route("api/terminal/{terminalId}/terminalsettings")]
    public class TerminalSettingController : ControllerBase
    {
        private readonly ITerminalSettingsRepository _terminalSettingsRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;


        public TerminalSettingController(ITerminalSettingsRepository terminalSettingsRepository,
            IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _terminalSettingsRepository = terminalSettingsRepository ??
             throw new ArgumentNullException(nameof(terminalSettingsRepository));
            _mapper = mapper ??
              throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [HttpGet(Name = "GetTerminalSettings")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 90)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        [Produces("application/json",
           "application/vnd.marvin.hateoas+json",
           "application/vnd.marvin.terminalsettings.full+json",
           "application/vnd.marvin.terminalsettings.full.hateoas+json",
           "application/vnd.marvin.terminalsettings.friendly+json",
           "application/vnd.marvin.terminalsettings.friendly.hateoas+json")]
        public IActionResult GetTerminalSettings(Guid terminalId,
            [FromQuery] TerminalSettingsParameters terminalSettingsParameters)
        {
            terminalSettingsParameters.TerminalId = terminalId;

            if (!_propertyMappingService.ValidTerminalSettingsMappingExistsFor<TerminalSettingsDto, Entities.TerminalSettings>
               (terminalSettingsParameters.OrderBy))
            {
                return BadRequest();
            }

            var terminalSettingsFromRepo = _terminalSettingsRepository.GetTerminalSettings(terminalSettingsParameters);

            var paginationMetadata = new
            {
                totalCount = terminalSettingsFromRepo.TotalCount,
                pageSize = terminalSettingsFromRepo.PageSize,
                currentPage = terminalSettingsFromRepo.CurrentPage,
                totalPages = terminalSettingsFromRepo.TotalPages

            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForTerminalSettings(terminalSettingsParameters,
                terminalSettingsFromRepo.HasNext,
                terminalSettingsFromRepo.HasPrevious);

            var shapedSites = _mapper.Map<IEnumerable<TerminalSettingsDto>>(terminalSettingsFromRepo)
                               .ShapeData(terminalSettingsParameters.Fields);

            var shapedTerminalWithLinks = shapedSites.Select(terminalSettings =>
            {
                var terminalSettingsAsDictionary = terminalSettings as IDictionary<string, object>;
                var terminalSettingsLinks = CreateLinksForTerminalSettings(terminalId.ToString(),(string)terminalSettingsAsDictionary["Id"], null);
                terminalSettingsAsDictionary.Add("links", terminalSettingsLinks);
                return terminalSettingsAsDictionary;
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
          "application/vnd.marvin.terminalsettingsforcreation.full+json",
          "application/vnd.marvin.terminalsettingsforcreation.full.hateoas+json",
          "application/vnd.marvin.terminalsettingsforcreation.friendly+json",
          "application/vnd.marvin.terminalsettingsforcreation.friendly.hateoas+json")]
        [HttpPost(Name = "CreateTerminalSettings")]
        [RequestHeaderMatchesMediaType("Content-Type",
           "application/json",
           "application/vnd.marvin.terminalsettingsforcreation+json")]
        [Consumes("application/json",
           "application/vnd.marvin.terminalsettingsforcreation+json")]
        public ActionResult<TerminalSettingsDto> CreateTerminalSettings(Guid terminalId,TerminalSettingsForCreationDto terminalSettings)
        {
            terminalSettings.TerminalId = terminalId.ToString();

            var terminalEntity = _mapper.Map<Entities.TerminalSettings>(terminalSettings);
            _terminalSettingsRepository.AddTerminalSettings(terminalId, terminalEntity);
            _terminalSettingsRepository.Save();

            var terminalToReturn = _mapper.Map<TerminalSettingsDto>(terminalEntity);

            var links = CreateLinksForTerminalSettings(terminalId.ToString(), terminalToReturn.TerminalId.ToString(), null);

            var linkedResourceToReturn = terminalToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetTerminalSettings",
                new { terminalId },
                linkedResourceToReturn);
        }

        [Produces("application/json",
          "application/vnd.marvin.hateoas+json",
          "application/vnd.marvin.terminalsettingsforpatch.full+json",
          "application/vnd.marvin.terminalsettingsforpatch.full.hateoas+json",
          "application/vnd.marvin.terminalsettingsforpatch.friendly+json",
          "application/vnd.marvin.terminalsettingsforpatch.friendly.hateoas+json")]
        [RequestHeaderMatchesMediaType("Content-Type",
          "application/json",
          "application/vnd.marvin.terminalsettingsforpatch+json")]
        [Consumes("application/json",
          "application/vnd.marvin.terminalsettingsforpatch+json")]
        [HttpPatch("{terminalSettingsId}", Name = "PatchTerminalSettings")]
        public ActionResult PatchTerminal(Guid terminalId, Guid terminalSettingsId, [FromBody]JsonPatchDocument<TerminalSettingsForUpdateDto> patchDocument)
        {

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_terminalSettingsRepository.TerminalSettingExists(terminalSettingsId))
            {
                return NotFound();
            }

            var terminalSettingsFromRepo = _terminalSettingsRepository.GetTerminalSettings(terminalSettingsId);

            if (terminalSettingsFromRepo == null)
            {
                var terminalSettingsDto = new TerminalSettingsForUpdateDto();
                patchDocument.ApplyTo(terminalSettingsDto, ModelState);

                if (!TryValidateModel(terminalSettingsDto))
                {
                    return ValidationProblem(ModelState);
                }

                var terminalSettingsToUpdate = _mapper.Map<Entities.TerminalSettings>(terminalSettingsDto);
                terminalSettingsToUpdate.Id = terminalSettingsId;

                _terminalSettingsRepository.UpdateTerminalSettings(terminalId, terminalSettingsToUpdate);

                _terminalSettingsRepository.Save();

                var terminalToReturn = _mapper.Map<TerminalDto>(terminalSettingsToUpdate);

                return CreatedAtRoute("GetTerminal",
                    new { terminalSettingsId }, terminalToReturn);
            }

            var terminalSettingsToPatch = _mapper.Map<TerminalSettingsForUpdateDto>(terminalSettingsFromRepo);

            patchDocument.ApplyTo(terminalSettingsToPatch);

            if (!TryValidateModel(terminalSettingsToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(terminalSettingsToPatch, terminalSettingsFromRepo);

            _terminalSettingsRepository.UpdateTerminalSettings(terminalSettingsId, terminalSettingsFromRepo);

            _terminalSettingsRepository.Save();

            return NoContent();

        }

        [Produces("application/json",
         "application/vnd.marvin.hateoas+json",
         "application/vnd.marvin.terminalsettingsfordelete.full+json",
         "application/vnd.marvin.terminalsettingsfordelete.full.hateoas+json",
         "application/vnd.marvin.terminalsettingsfordelete.friendly+json",
         "application/vnd.marvin.terminalsettingsfordelete.friendly.hateoas+json")]
        [HttpDelete("{terminalsettings}", Name = "DeleteTerminalSettings")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.deleteterminalsettings+json")]
        [Consumes("application/json",
            "application/vnd.marvin.deleteterminalsettings+json")]

        public ActionResult DeleteTerminalSettings(Guid terminalId)
        {
            var terminalSettingsFromRepo = _terminalSettingsRepository.GetTerminalSettings(terminalId);

            if (terminalSettingsFromRepo == null)
            {
                return NotFound();
            }

            _terminalSettingsRepository.DeleteTerminalSettings(terminalSettingsFromRepo);

            _terminalSettingsRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetTerminalSettingsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,DELETE,PATCH");
            return Ok();
        }
        private string CreateTerminalSettingsResourceUri(
          TerminalSettingsParameters terminalSettingsParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetTerminalSettings",
                      new
                      {
                          fields = terminalSettingsParameters.Fields,
                          orderBy = terminalSettingsParameters.OrderBy,
                          pageNumber = terminalSettingsParameters.PageNumber - 1,
                          pageSize = terminalSettingsParameters.PageSize,
                          searchQuery = terminalSettingsParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetTerminalSettings",
                      new
                      {
                          fields = terminalSettingsParameters.Fields,
                          orderBy = terminalSettingsParameters.OrderBy,
                          pageNumber = terminalSettingsParameters.PageNumber + 1,
                          pageSize = terminalSettingsParameters.PageSize,
                          searchQuery = terminalSettingsParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetTerminalSettings",
                    new
                    {
                        fields = terminalSettingsParameters.Fields,
                        orderBy = terminalSettingsParameters.OrderBy,
                        pageNumber = terminalSettingsParameters.PageNumber,
                        pageSize = terminalSettingsParameters.PageSize,
                        searchQuery = terminalSettingsParameters.SearchQuery
                    });
            }

        }
        private IEnumerable<LinkDto> CreateLinksForTerminalSettings(string terminalId,string terminalSettingsId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetTerminalSettings", new { terminalId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetTerminalSettings", new { terminalId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("DeleteTerminalSettings", new { terminalSettingsId }),
                   "delete_terminalsettings",
                   "DELETE"));
                links.Add(
                   new LinkDto(Url.Link("PatchTerminalSettings", new { terminalId, terminalSettingsId }),
                   "patch_terminal_settings",
                   "PATCH"));
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;
            }

            return links;
        }
        
        private IEnumerable<LinkDto> CreateLinksForTerminalSettings(
        TerminalSettingsParameters terminalSettingsParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateTerminalSettingsResourceUri(
                   terminalSettingsParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateTerminalSettingsResourceUri(
                      terminalSettingsParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateTerminalSettingsResourceUri(
                        terminalSettingsParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
