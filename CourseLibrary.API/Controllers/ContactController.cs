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
using System.Security.Policy;

namespace Siteminder.API.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId}")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class ContactController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public ContactController(IContactRepository contactRepository, IMapper mapper, IPropertyMappingService propertyMappingService,
            IPropertyCheckerService propertyCheckerService)
        {
            _contactRepository = contactRepository ??
              throw new ArgumentNullException(nameof(contactRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
              throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ??
              throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        [HttpGet("contacts", Name = "GetContacts")]
        [Produces("application/json",
          "application/vnd.marvin.hateoas+json",
          "application/vnd.marvin.getcontacts.full+json",
          "application/vnd.marvin.getcontacts.full.hateoas+json",
          "application/vnd.marvin.getcontacts.friendly+json",
          "application/vnd.marvin.getcontacts.friendly.hateoas+json")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = true)]
        [HttpHead]
        public IActionResult GetContacts(Guid companyId, Guid siteTypeId, Guid siteId,
            [FromQuery] ContactResourceParameters contactResourceParameters)
        {
            if (!_propertyMappingService.ValidSiteMappingExistsFor<ContactDto, Entities.Contact>
               (contactResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var contactsFromRepo = _contactRepository.GetCompanyContacts(companyId, contactResourceParameters);

            var paginationMetadata = new
            {
                totalCount = contactsFromRepo.TotalCount,
                pageSize = contactsFromRepo.PageSize,
                currentPage = contactsFromRepo.CurrentPage,
                totalPages = contactsFromRepo.TotalPages
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForSiteContact(contactResourceParameters,
                contactsFromRepo.HasNext,
                contactsFromRepo.HasPrevious);

            var shapedContacts = _mapper.Map<IEnumerable<ContactDto>>(contactsFromRepo)
                               .ShapeData(contactResourceParameters.Fields);

            var shapedContactsWithLinks = shapedContacts.Select(contacts =>
            {
                var siteAsDictionary = contacts as IDictionary<string, object>;
                var siteLinks = CreateLinksForCompanyContacts(companyId.ToString(), (string)siteAsDictionary["Id"], null);
                siteAsDictionary.Add("links", siteLinks);
                return siteAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedContactsWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
        }

        [HttpGet("contacts/{contactId}", Name = "GetContactForCompany")]
        [Produces("application/json",
          "application/vnd.marvin.hateoas+json",
          "application/vnd.marvin.getcontactforcompany.full+json",
          "application/vnd.marvin.getcontactforcompany.full.hateoas+json",
          "application/vnd.marvin.getcontactforcompany.friendly+json",
          "application/vnd.marvin.getcontactforcompany.friendly.hateoas+json")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.getcontactforcompany+json")]
        [Consumes("application/json",
            "application/vnd.marvin.getcontactforcompany+json")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetContactForCompany(Guid companyId, Guid siteTypeId, Guid siteId, Guid contactId, string fields,
               [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<ContactDto>
               (fields))
            {
                return BadRequest();
            }

            var contactFromRepo = _contactRepository.GetCompanyContact(companyId, contactId);

            if (contactFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForCompanyContacts(companyId.ToString(), contactId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full Site
            if (primaryMediaType == "vnd.marvin.contacts.full")
            {
                var fullResourceToReturn = _mapper.Map<ContactFullDto>(contactFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly site
            var friendlyResourceToReturn = _mapper.Map<ContactDto>(contactFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

        [HttpGet("contacts/{contactId}", Name = "GetContactForSite")]
        [Produces("application/json",
        "application/vnd.marvin.hateoas+json",
        "application/vnd.marvin.getcontactforsite.full+json",
        "application/vnd.marvin.getcontactforsite.full.hateoas+json",
        "application/vnd.marvin.getcontactforsite.friendly+json",
        "application/vnd.marvin.getcontactforsite.friendly.hateoas+json")]
        [RequestHeaderMatchesMediaType("Content-Type",
          "application/json",
          "application/vnd.marvin.getcontactforsite+json")]
        [Consumes("application/json",
          "application/vnd.marvin.getcontactforsite+json")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public IActionResult GetContactForSite(Guid companyId, Guid siteId, Guid contactId, string fields,
             [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType,
               out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<ContactDto>
               (fields))
            {
                return BadRequest();
            }

            var contactFromRepo = _contactRepository.GetCompanySiteContact(companyId, siteId, contactId);

            if (contactFromRepo == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
              .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();

            if (includeLinks)
            {
                links = CreateLinksForCompanyContacts(companyId.ToString(), contactId.ToString(), fields);
            }

            var primaryMediaType = includeLinks ?
              parsedMediaType.SubTypeWithoutSuffix
              .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
              : parsedMediaType.SubTypeWithoutSuffix;

            // full Site
            if (primaryMediaType == "vnd.marvin.contacts.full")
            {
                var fullResourceToReturn = _mapper.Map<ContactFullDto>(contactFromRepo)
                    .ShapeData(fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly site
            var friendlyResourceToReturn = _mapper.Map<ContactDto>(contactFromRepo)
                .ShapeData(fields) as IDictionary<string, object>;

            if (includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

        }

        [HttpPost("contacts",Name = "CreateContactForCompany")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.createcontactforcompany+json")]
        [Consumes("application/json",
            "application/vnd.marvin.createcontactforcompany+json")]
        public ActionResult<ContactDto> CreateContactForCompany(Guid companyId, Guid siteTypeId, Guid siteId, ContactForCreationDto contact)
        {

            var contactEntity = _mapper.Map<Entities.Contact>(contact);

            _contactRepository.CreateCompanyContact(companyId,contactEntity);
            _contactRepository.Save();

            var contactToReturn = _mapper.Map<ContactDto>(contactEntity);

            var links = CreateLinksForCompanyContacts(companyId.ToString(), contactToReturn.Id, null);

            var linkedResourceToReturn = contactToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetContactForCompany",
                new { companyId, contactId = linkedResourceToReturn["Id"] },
                linkedResourceToReturn);
        }

        [HttpPost("sites/{siteId}/contacts/{contactId}",Name = "CreateContactForSite")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.createcontactforsite+json")]
        [Consumes("application/json",
            "application/vnd.marvin.createcontactforsite+json")]
        public ActionResult<ContactDto> CreateContactForSite(Guid companyId, Guid siteId, Guid contactId, SiteContactForCreationDto siteContact)
        {
            if (companyId == null||  siteId == null || contactId == null)
            {
                return BadRequest();
            }

            if (!_contactRepository.ContactExists(contactId))
            {
                return BadRequest();
            }

            var contactEntity = _mapper.Map<Entities.SiteContacts>(siteContact);


            contactEntity.ContactId = contactId;
            contactEntity.SiteId = siteId;

            _contactRepository.CreateCompanySiteContact(contactEntity);
            _contactRepository.Save();

            var contactToReturn = _mapper.Map<SiteContactForCreationDto>(contactEntity);

            var links = CreateLinksForSiteContacts(companyId.ToString(),siteId.ToString(),contactToReturn.ContactId.ToString(), null);

            var linkedResourceToReturn = contactToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetContactForCompany",
               new { companyId, contactId },
               linkedResourceToReturn);
        }

        [HttpPost("terminals/{terminalId}/contacts", Name = "CreateContactForTerminal")]
        [RequestHeaderMatchesMediaType("Content-Type",
         "application/json",
         "application/vnd.marvin.createcontactforterminal+json")]
        [Consumes("application/json",
         "application/vnd.marvin.createcontactforterminal+json")]
        public ActionResult<ContactDto> CreateContactForTerminal(Guid companyId, Guid terminalId, Guid contactId,TerminalContactForCreationDto TerminalContact)
        {
            if (terminalId == null || contactId == null)
            {
                return BadRequest();
            }

            if (!_contactRepository.ContactExists(contactId))
            {
                return BadRequest();
            }

            var contactEntity = _mapper.Map<Entities.TerminalContacts>(TerminalContact);

            contactEntity.ContactId = contactId;
            contactEntity.TerminalId = terminalId;

            _contactRepository.CreateCompanySiteTerminalContact(contactEntity);
            _contactRepository.Save();

            var contactToReturn = _mapper.Map<ContactDto>(contactEntity);

            var links = CreateLinksForTerminalContacts(companyId.ToString(),terminalId.ToString(), contactToReturn.Id, null);

            var linkedResourceToReturn = contactToReturn.ShapeData(null)
                as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetContactForCompany",
                new { companyId, contactId },
                linkedResourceToReturn);
        }

        [HttpPatch("contacts/{contactId}", Name = "PatchContact")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.patchcontact+json")]
        [Consumes("application/json",
           "application/vnd.marvin.patchcontact+json")]
        public ActionResult PatchContact(Guid companyId, Guid contactId, [FromBody]JsonPatchDocument<ContactForUpdateDto> patchDocument)
        {

            if (contactId == null)
            {
                return BadRequest();
            }

            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_contactRepository.ContactExists(contactId))
            {
                return NotFound();
            }

            Contact contactFromRepo = _contactRepository.GetCompanyContact(companyId, contactId);

            if (contactFromRepo == null)
            {
                var contactDto = new ContactForUpdateDto();
                patchDocument.ApplyTo(contactDto, ModelState);

                if (!TryValidateModel(contactDto))
                {
                    return ValidationProblem(ModelState);
                }

                var contactToAdd = _mapper.Map<Entities.Contact>(contactDto);
                contactToAdd.Id = contactId;

                _contactRepository.CreateCompanyContact(companyId, contactToAdd);

                _contactRepository.Save();

                var contactToReturn = _mapper.Map<ContactDto>(contactToAdd);

                return CreatedAtRoute("GetContact",
                    new { contactId }, contactToReturn);
            }

            var contactToPatch = _mapper.Map<ContactForUpdateDto>(contactFromRepo);

            patchDocument.ApplyTo(contactToPatch, ModelState); ;

            if (!TryValidateModel(contactToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(contactToPatch, contactFromRepo);

            _contactRepository.PatchContact(contactFromRepo);

            _contactRepository.Save();

            return NoContent();
        }

        [HttpDelete("contacts/{contactId}", Name = "DeleteContactFromCompany")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.deletecontactfromcompany+json")]
        [Consumes("application/json",
            "application/vnd.marvin.deletecontactfromcompany+json")]
        public ActionResult DeleteContactFromCompany(Guid companyId, Guid contactId)
        {
            var contactFromRepo = _contactRepository.GetCompanyContact(companyId, contactId);

            if (contactFromRepo == null)
            {
                return NotFound();
            }

            _contactRepository.DeleteCompanyContact(contactFromRepo);

            _contactRepository.Save();

            return NoContent();
        }

        [HttpDelete("sites/{siteId}/contacts/{contactId}", Name = "DeleteContactFromSite")]
        [RequestHeaderMatchesMediaType("Content-Type",
           "application/json",
           "application/vnd.marvin.deletecontactfromsite+json")]
        [Consumes("application/json",
           "application/vnd.marvin.deletecontactfromsite+json")]
        public ActionResult DeleteContactFromSite(Guid companyId, Guid siteId, Guid contactId)
        {
            var contactFromRepo = _contactRepository.GetCompanySiteContact(companyId, siteId, contactId);

            if (contactFromRepo == null)
            {
                return NotFound();
            }

            _contactRepository.DeleteSiteContactsAssignment(contactFromRepo);

            _contactRepository.Save();

            return NoContent();
        }

        [HttpDelete("terminals/{terminalId}/contacts/{contactId}", Name = "DeleteContactFromTerminal")]
        [RequestHeaderMatchesMediaType("Content-Type",
          "application/json",
          "application/vnd.marvin.deletecontactfromterminal+json")]
        [Consumes("application/json",
          "application/vnd.marvin.deletecontactfromterminal+json")]
        public ActionResult DeleteContactFromTerminal(Guid companyId, Guid terminalId, Guid contactId)
        {
            var contactFromRepo = _contactRepository.GetCompanySiteTerminalContact(companyId, terminalId, contactId);

            if (contactFromRepo == null)
            {
                return NotFound();
            }

            _contactRepository.DeleteTerminalContactsAssignment(contactFromRepo);

            _contactRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetContactOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST,PATCH");
            return Ok();
        }
        private string CreateContactResourceUri(
          ContactResourceParameters contactResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetSites",
                      new
                      {
                          fields = contactResourceParameters.Fields,
                          orderBy = contactResourceParameters.OrderBy,
                          pageNumber = contactResourceParameters.PageNumber - 1,
                          pageSize = contactResourceParameters.PageSize,
                          searchQuery = contactResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetSites",
                      new
                      {
                          fields = contactResourceParameters.Fields,
                          orderBy = contactResourceParameters.OrderBy,
                          pageNumber = contactResourceParameters.PageNumber + 1,
                          pageSize = contactResourceParameters.PageSize,
                          searchQuery = contactResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetSites",
                    new
                    {
                        fields = contactResourceParameters.Fields,
                        orderBy = contactResourceParameters.OrderBy,
                        pageNumber = contactResourceParameters.PageNumber,
                        pageSize = contactResourceParameters.PageSize,
                        searchQuery = contactResourceParameters.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForCompanyContacts(string companyId, string contactId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {
                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetContactForCompany", new { companyId, contactId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetContactForCompany", new { companyId,  contactId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("PatchContact", new { companyId,  contactId }),
                   "patch_contact",
                   "PATCH"));

                links.Add(
                   new LinkDto(Url.Link("DeleteContactFromCompany", new { companyId, contactId }),
                   "delete_contact",
                   "DELETE"));

            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForSiteContacts(string companyId, string siteId, string contactId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {
                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetContactForSite", new { companyId, siteId, contactId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetContactForSite", new { companyId,siteId, contactId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("PatchContact", new { companyId, contactId }),
                   "patch_contact",
                   "PATCH"));

                links.Add(
                   new LinkDto(Url.Link("DeleteContactFromCompany", new { companyId, contactId }),
                   "delete_contact",
                   "DELETE"));

            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForSiteContact(
        ContactResourceParameters contactResourceParameters,
        bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateContactResourceUri(
                   contactResourceParameters, ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateContactResourceUri(
                      contactResourceParameters, ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateContactResourceUri(
                        contactResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForTerminalContacts(string companyId, string terminalId, string contactId, string fields)
        {
            var links = new List<LinkDto>();

            try
            {

                if (string.IsNullOrWhiteSpace(fields))
                {
                    links.Add(
                      new LinkDto(Url.Link("GetContactForCompany", new { companyId, terminalId, contactId }),
                      "self",
                      "GET"));
                }
                else
                {
                    links.Add(
                      new LinkDto(Url.Link("GetContactForCompany", new { companyId, terminalId, contactId, fields }),
                      "self",
                      "GET"));
                }

                links.Add(
                   new LinkDto(Url.Link("PatchContact", new { companyId, terminalId, contactId, }),
                   "patch_contact",
                   "PATCH"));

                links.Add(
                   new LinkDto(Url.Link("DeleteContactFromCompany", new { companyId, terminalId, contactId, }),
                   "delete_contact",
                   "DELETE"));

            }
            catch (Exception ex)
            {
                var errMsg = ex.Message;

            }

            return links;
        }

    }
}