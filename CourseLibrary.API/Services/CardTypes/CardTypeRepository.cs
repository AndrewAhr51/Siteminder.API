using Siteminder.API.DbContexts;
using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.Models;
using Siteminder.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public class CardTypeRepository : ICardTypeRepository, IDisposable
    {
        private readonly SiteminderContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public CardTypeRepository(SiteminderContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new Exception(nameof(context));

            _propertyMappingService = propertyMappingService ??
               throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddCardType(CardType cardType)
        {
            if (cardType == null)
            {
                throw new ArgumentNullException(nameof(cardType));
            }

            cardType.Id = Guid.NewGuid();

            _context.CardTypes.Add(cardType);
        }

        public void AddCardType(Guid cardTypeId, CardType cardType)
        {
            if (cardType == null)
            {
                throw new ArgumentNullException(nameof(cardType));
            }

            cardType.Id = cardTypeId;

            _context.CardTypes.Add(cardType);
        }

        public bool CardTypeExists(Guid cardTypeId)
        {
            if (cardTypeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(cardTypeId));
            }

            return _context.CardTypes.Any(a => a.Id == cardTypeId);
        }

        public void DeleteCardType(CardType cardType)
        {
            if (cardType == null)
            {
                throw new ArgumentNullException(nameof(cardType));
            }

            _context.CardTypes.Remove(cardType);
        }

        public PagedList<CardType> GetCardTypes(CardTypeResourceParameters cardTypeResourseParameters)
        {
            if (cardTypeResourseParameters == null)
            {
                throw new ArgumentNullException(nameof(cardTypeResourseParameters));
            }

            var collection = _context.Company as IQueryable<CardType>;


            if (!string.IsNullOrWhiteSpace(cardTypeResourseParameters.SearchQuery))
            {
                var searchQuery = cardTypeResourseParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(cardTypeResourseParameters.OrderBy))
            {
                var cardTypePropertyMappingDictionary = _propertyMappingService.GetCardTypePropertyMapping<CardTypeDto, CardType>();

                collection = collection.ApplySort(cardTypeResourseParameters.OrderBy, cardTypePropertyMappingDictionary);

            }

            //Paging.... happens LAST
            return PagedList<CardType>.Create(collection,
                cardTypeResourseParameters.PageNumber,
                cardTypeResourseParameters.PageSize);
        }

        public CardType GetCardType(Guid cardTypeId)
        {
            if (cardTypeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(cardTypeId));
            }

            return _context.CardTypes.FirstOrDefault(a => a.Id == cardTypeId);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void PatchCardType(CardType cardType)
        {
            if (cardType == null)
            {
                throw new ArgumentNullException(nameof(cardType));
            }

            _context.CardTypes.Update(cardType);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
