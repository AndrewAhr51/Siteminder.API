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
    [Route("api/terminals/{terminalId}/devices")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public DeviceController(IDeviceRepository deviceRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _deviceRepository = deviceRepository ??
            throw new ArgumentNullException(nameof(deviceRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [Produces("application/json",
        "application/vnd.marvin.hateoas+json",
        "application/vnd.marvin.getdevicesforterminal.full+json",
        "application/vnd.marvin.getdevicesforterminal.full.hateoas+json",
        "application/vnd.marvin.getdevicesforterminal.friendly+json",
        "application/vnd.marvin.getdevicesforterminal.friendly.hateoas+json")]
        [HttpGet(Name = "GetDevices")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        public IActionResult GetDevices(Guid terminalId,
       [FromQuery] DeviceResourceParameters deviceResourceParameters)
        {
            if (!_propertyMappingService.ValidDeviceMappingExistsFor<DevicesDto, Entities.Device>
               (deviceResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            deviceResourceParameters.TerminalId = terminalId;

            var devicesFromRepo = _deviceRepository.GetDevices(deviceResourceParameters);

            var paginationMetadata = new
            {
                totalCount = devicesFromRepo.TotalCount,
                pageSize = devicesFromRepo.PageSize,
                currentPage = devicesFromRepo.CurrentPage,
                totalPages = devicesFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForDevice(deviceResourceParameters,
                devicesFromRepo.HasNext,
                devicesFromRepo.HasPrevious);

            var shapedDevices = _mapper.Map<IEnumerable<DevicesDto>>(devicesFromRepo)
                               .ShapeData(deviceResourceParameters.Fields);

            var shapedDevicesWithLinks = shapedDevices.Select(devices =>
            {
                var deviceAsDictionary = devices as IDictionary<string, object>;
                var deviceLinks = CreateLinksForDevice(terminalId.ToString(), (string)deviceAsDictionary["Id"], null);
                deviceAsDictionary.Add("links", deviceLinks);
                return deviceAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedDevicesWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [Produces("application/json",
          "application/vnd.marvin.hateoas+json",
          "application/vnd.marvin.getdeviceforterminal.full+json",
          "application/vnd.marvin.getdeviceforterminal.full.hateoas+json",
          "application/vnd.marvin.getdeviceforterminal.friendly+json",
          "application/vnd.marvin.getdeviceforterminal.friendly.hateoas+json")]
        [HttpGet("{deviceId}", Name = "GetDevice")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.getdeviceforterminal+json")]
        [Consumes("application/json",
            "application/vnd.marvin.getdeviceforterminal+json")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetDevice(Guid terminalId, Guid deviceId, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<DevicesDto>
               (fields))
            {
                return BadRequest();
            }

            var devicesFromRepo = _deviceRepository.GetDevices(terminalId, deviceId);

            if (devicesFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForDevice(terminalId.ToString(), deviceId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full Site
            if (primaryMediaType == "vnd.marvin.device.full")
            {
                var fullResourceToReturn = _mapper.Map<DevicesFullDto>(devicesFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly device
            var friendlyResourceToReturn = _mapper.Map<DevicesDto>(devicesFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

        [HttpPost(Name = "CreateDeviceForTerminal")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.createdeviceforterminal+json")]
        [Consumes("application/json",
            "application/vnd.marvin.createdeviceforterminal+json")]
        public ActionResult<DevicesDto> CreateDeviceForTerminal(Guid terminalId, DevicesForCreationDto device)
        {

            var deviceEntity = _mapper.Map<Entities.Device>(device);
            deviceEntity.TerminalId = terminalId;
            //deviceEntity.Id = Guid.NewGuid();

            _deviceRepository.AddDevice(deviceEntity);
            _deviceRepository.Save();

            var deviceToReturn = _mapper.Map<DevicesDto>(deviceEntity);

            var links = CreateLinksForDevice(terminalId.ToString(), deviceToReturn.Id, null);

            var linkedResourceToReturn = deviceToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetDevices",
                new { terminalId, deviceId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.deviceforpatch+json")]
        [Consumes("application/json",
           "application/vnd.marvin.deviceforpatch+json")]
        [HttpPatch("{deviceId}", Name = "PatchDevice")]
        public ActionResult PatchDevice(Guid terminalId, Guid deviceId, [FromBody]JsonPatchDocument<DevicesForUpdateDto> patchDocument)
        {

            if (terminalId == Guid.Empty || deviceId == Guid.Empty)
            {
                return BadRequest();
            }

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_deviceRepository.DeviceExists(deviceId))
            {
                return NotFound();
            }

            var deviceFromRepo = _deviceRepository.GetDevices(terminalId, deviceId);

            if (deviceFromRepo == null)
            {
                var deviceDto = new DevicesForUpdateDto();
                patchDocument.ApplyTo(deviceDto, ModelState);

                if (!TryValidateModel(deviceDto))
                {
                    return ValidationProblem(ModelState);
                }

                var deviceToAdd = _mapper.Map<Entities.Device>(deviceDto);
                deviceToAdd.TerminalId = terminalId;

                _deviceRepository.AddDevice(terminalId, deviceToAdd);

                _deviceRepository.Save();

                var deviceToReturn = _mapper.Map<DevicesDto>(deviceToAdd);

                return CreatedAtRoute("GetDevice",
                    new { deviceId }, deviceToReturn);
            }

            var deviceToPatch = _mapper.Map<DevicesForUpdateDto>(deviceFromRepo);

            patchDocument.ApplyTo(deviceToPatch);

            if (!TryValidateModel(deviceToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(deviceToPatch, deviceFromRepo);

            _deviceRepository.UpdateDevice(deviceFromRepo);

            _deviceRepository.Save();

            return NoContent();
        }

        [HttpDelete("{deviceId}", Name = "DeleteDevice")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.deletedevice+json")]
        [Consumes("application/json",
            "application/vnd.marvin.deletedevice+json")]
        public ActionResult DeleteSite(Guid terminalId, Guid deviceId)
        {
            var deviceFromRepo = _deviceRepository.GetDevices(terminalId, deviceId);

            if (deviceFromRepo == null)
            {
                return NotFound();
            }

            _deviceRepository.DeleteDevice(deviceFromRepo);

            _deviceRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetDeviceOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST, PATCH");
            return Ok();
        }
        private string CreateDevicesResourceUri(
          DeviceResourceParameters deviceResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetDevices",
                      new
                      {
                          fields = deviceResourceParameters.Fields,
                          orderBy = deviceResourceParameters.OrderBy,
                          pageNumber = deviceResourceParameters.PageNumber - 1,
                          pageSize = deviceResourceParameters.PageSize,
                          searchQuery = deviceResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetDevices",
                      new
                      {
                          fields = deviceResourceParameters.Fields,
                          orderBy = deviceResourceParameters.OrderBy,
                          pageNumber = deviceResourceParameters.PageNumber + 1,
                          pageSize = deviceResourceParameters.PageSize,
                          searchQuery = deviceResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetDevices",
                    new
                    {
                        fields = deviceResourceParameters.Fields,
                        orderBy = deviceResourceParameters.OrderBy,
                        pageNumber = deviceResourceParameters.PageNumber,
                        pageSize = deviceResourceParameters.PageSize,
                        searchQuery = deviceResourceParameters.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForDevice(string terminalId, string deviceId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetDevice", new { terminalId, deviceId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetDevice", new { terminalId, deviceId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("PatchDevice", new { terminalId, deviceId }),
                   "patch_device",
                   "PATCH"));

                links.Add(
                   new LinkDto(Url.Link("DeleteDevice", new { terminalId, deviceId }),
                   "delete_device",
                   "DELETE"));

            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForDevice(
        DeviceResourceParameters deviceResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateDevicesResourceUri(
                   deviceResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateDevicesResourceUri(
                      deviceResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateDevicesResourceUri(
                        deviceResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }

}