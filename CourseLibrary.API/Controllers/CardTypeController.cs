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
    [Route("api/cardtypes")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class CardTypeController : ControllerBase
    {
        private readonly ICardTypeRepository _cardTypeRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public CardTypeController(ICardTypeRepository cardTypeRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
          IPropertyCheckerService propertyCheckerService)
        {
            _cardTypeRepository = cardTypeRepository ??
               throw new ArgumentNullException(nameof(cardTypeRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }


        [Produces("application/json",
           "application/vnd.marvin.hateoas+json",
           "application/vnd.marvin.getcardtypes.full+json",
           "application/vnd.marvin.getcardtypes.full.hateoas+json",
           "application/vnd.marvin.getcardtypes.friendly+json",
           "application/vnd.marvin.getcardtypes.friendly.hateoas+json")]
        [HttpGet(Name = "GetCardTypes")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpHead]
        public IActionResult GetCardTypes(
            [FromQuery] CardTypeResourceParameters cardTypeResourceParameters)
        {
            if (!_propertyMappingService.ValidCardTypeMappingExistsFor<CardTypeDto, Entities.CardType>
               (cardTypeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var cardTypeFromRepo = _cardTypeRepository.GetCardTypes(cardTypeResourceParameters);

            var paginationMetadata = new
            {
                totalCount = cardTypeFromRepo.TotalCount,
                pageSize = cardTypeFromRepo.PageSize,
                currentPage = cardTypeFromRepo.CurrentPage,
                totalPages = cardTypeFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForCardType(cardTypeResourceParameters,
                cardTypeFromRepo.HasNext,
                cardTypeFromRepo.HasPrevious);

            var shapedCompanies = _mapper.Map<IEnumerable<CardTypeDto>>(cardTypeFromRepo)
                               .ShapeData(cardTypeResourceParameters.Fields);

            var shapedCompaniesWithLinks = shapedCompanies.Select(cardType =>
            {
                var cardTypeAsDictionary = cardType as IDictionary<string, object>;
                var cardTypeLinks = CreateLinksForCardType((string)cardTypeAsDictionary["Id"], null);
                cardTypeAsDictionary.Add("links", cardTypeLinks);
                return cardTypeAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedCompaniesWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [HttpPost(Name = "CreateCardType")]
        [RequestHeaderMatchesMediaType("Content-Type",
          "application/json",
          "application/vnd.marvin.cardtypeforcreation+json")]
        [Consumes("application/json",
          "application/vnd.marvin.cardtypeforcreation+json")]
        public ActionResult<CardTypeDto> CreateCardType(CardTypeForCreationDto CardType)
        {
            var CardTypeEntity = _mapper.Map<Entities.CardType>(CardType);
            _cardTypeRepository.AddCardType(CardTypeEntity);
            _cardTypeRepository.Save();

            var CardTypeToReturn = _mapper.Map<CardTypeDto>(CardTypeEntity);

            var links = CreateLinksForCardType(CardTypeToReturn.Id, null);

            var linkedResourceToReturn = CardTypeToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetCardType",
                new { CardTypeId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }


        [Produces("application/json",
          "application/vnd.marvin.hateoas+json",
          "application/vnd.marvin.getcardtype.full+json",
          "application/vnd.marvin.getcardtype.full.hateoas+json",
          "application/vnd.marvin.getcardtype.friendly+json",
          "application/vnd.marvin.getcardtype.friendly.hateoas+json")]
        [HttpGet("{cardTypeId}", Name = "GetCardType")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetCardType(Guid cardTypeId, string fields,
          [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<CardTypeDto>
               (fields))
            {
                return BadRequest();
            }

            var cardTypeFromRepo = _cardTypeRepository.GetCardType(cardTypeId);

            if (cardTypeFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForCardType(cardTypeId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full author
            if (primaryMediaType == "vnd.marvin.cardtype.full")
            {
                var fullResourceToReturn = _mapper.Map<CardTypeFullDto>(cardTypeFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly author
            var friendlyResourceToReturn = _mapper.Map<CardTypeDto>(cardTypeFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

        [RequestHeaderMatchesMediaType("Content-Type",
          "application/json",
          "application/vnd.marvin.cardtypeforpatch+json")]
        [Consumes("application/json",
          "application/vnd.marvin.cardtypeforpatch+json")]
        [HttpPatch("{cardTypeId}", Name = "PatchCardType")]
        public ActionResult PatchCardType(Guid cardTypeId, [FromBody]JsonPatchDocument<CardTypeForUpdateDto> patchDocument)
        {

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_cardTypeRepository.CardTypeExists(cardTypeId))
            {
                return NotFound();
            }

            var cardTypeFromRepo = _cardTypeRepository.GetCardType(cardTypeId);

            if (cardTypeFromRepo == null)
            {
                var cardTypeDto = new CardTypeForUpdateDto();
                patchDocument.ApplyTo(cardTypeDto, ModelState);

                if (!TryValidateModel(cardTypeDto))
                {
                    return ValidationProblem(ModelState);
                }

                var cardTypeToAdd = _mapper.Map<Entities.CardType>(cardTypeDto);
                cardTypeToAdd.Id = cardTypeId;

                _cardTypeRepository.AddCardType(cardTypeId, cardTypeToAdd);

                _cardTypeRepository.Save();

                var cardTypeToReturn = _mapper.Map<CardTypeDto>(cardTypeToAdd);

                return CreatedAtRoute("GetCardType",
                    new { cardTypeId }, cardTypeToReturn);
            }

            var cardTypeToPatch = _mapper.Map<CardTypeForUpdateDto>(cardTypeFromRepo);

            patchDocument.ApplyTo(cardTypeToPatch, ModelState);

            if (!TryValidateModel(cardTypeToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(cardTypeToPatch, cardTypeFromRepo);

            _cardTypeRepository.PatchCardType(cardTypeFromRepo);

            _cardTypeRepository.Save();

            return NoContent();

        }

        [HttpDelete("{cardTypeId}", Name = "DeleteCardType")]
        [RequestHeaderMatchesMediaType("Content-Type",
         "application/json",
         "application/vnd.marvin.cardTypefordeletion+json")]
        [Consumes("application/json",
         "application/vnd.marvin.cardTypefordeletion+json")]
        public ActionResult DeleteCardType(Guid cardTypeId)
        {
            var cardTypeFromRepo = _cardTypeRepository.GetCardType(cardTypeId);

            if (cardTypeFromRepo == null)
            {
                return NotFound();
            }

            _cardTypeRepository.DeleteCardType(cardTypeFromRepo);

            _cardTypeRepository.Save();

            return NoContent();
        }


        [HttpOptions]
        public IActionResult GetCardTypeOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PATCH");
            return Ok();
        }
        private string CreateCardTypeResourceUri(
          CardTypeResourceParameters cardTypeResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetCardTypes",
                      new
                      {
                          fields = cardTypeResourceParameters.Fields,
                          orderBy = cardTypeResourceParameters.OrderBy,
                          pageNumber = cardTypeResourceParameters.PageNumber - 1,
                          pageSize = cardTypeResourceParameters.PageSize,
                          searchQuery = cardTypeResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetCardTypes",
                      new
                      {
                          fields = cardTypeResourceParameters.Fields,
                          orderBy = cardTypeResourceParameters.OrderBy,
                          pageNumber = cardTypeResourceParameters.PageNumber + 1,
                          pageSize = cardTypeResourceParameters.PageSize,
                          searchQuery = cardTypeResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetCardTypes",
                    new
                    {
                        fields = cardTypeResourceParameters.Fields,
                        orderBy = cardTypeResourceParameters.OrderBy,
                        pageNumber = cardTypeResourceParameters.PageNumber,
                        pageSize = cardTypeResourceParameters.PageSize,
                        searchQuery = cardTypeResourceParameters.SearchQuery
                    });
            }

        }

        private IEnumerable<LinkDto> CreateLinksForCardType(string cardTypeId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetCardType", new { cardTypeId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetCardType", new { cardTypeId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("DeleteCardType", new { cardTypeId }),
                   "delete_cardtype",
                   "DELETE"));

                links.Add(
                   new LinkDto(Url.Link("PatchCardType", new { cardTypeId }),
                   "patch_cardtype",
                   "PATCH"));
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCardType(
        CardTypeResourceParameters cardTypeResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateCardTypeResourceUri(
                   cardTypeResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateCardTypeResourceUri(
                      cardTypeResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateCardTypeResourceUri(
                        cardTypeResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
    }
}
