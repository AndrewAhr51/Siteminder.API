using Siteminder.API.Entities;
using Siteminder.API.ResourceParameters;
using Siteminder.API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public interface ICardTypeRepository
    {
        void AddCardType(CardType cardType);
        void AddCardType(Guid cardTypeId, CardType cardType);
        bool CardTypeExists(Guid cardTypeId);
        void DeleteCardType(CardType cardType);
        public PagedList<CardType> GetCardTypes(CardTypeResourceParameters cardTypeResourseParameters);
        CardType GetCardType(Guid cardTypeId);
        void PatchCardType(CardType cardType);
        bool Save();
    }
}
