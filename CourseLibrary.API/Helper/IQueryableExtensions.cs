using Siteminder.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Siteminder.API.Helper
{
    public static  class IQueryableExtensions
    {

        public static IQueryable<T> ApplySort<T> (this IQueryable<T> source, string orderBy,
            Dictionary<string, PropertyMappingValue>mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            var orderbyAfterSplit = orderBy.Split(',');

            //applyeach orderby clause in reverse order- otherwise, the
            //IQueryablewill be ordered in the wrong order
            foreach(var orderbyClause in orderbyAfterSplit.Reverse())
            {
                //trim the orderby clause, as it might contain leading
                //or trailing spaces.  Can't trim the var in foreach,
                //so use another variable
                var trimmedOrderByClause = orderbyClause.Trim();

                //if the sort option ends with " desc", we order 
                //descending, otherwise ascending...
                var orderDescending = trimmedOrderByClause.EndsWith(" desc", System.StringComparison.OrdinalIgnoreCase);

                //remove " asc" or " desc" from the orderByClause, so we
                //get the propertyname to look for in the mapping dictionary...
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ", StringComparison.OrdinalIgnoreCase);
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                //find the matching property
                if(!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }

                //get the propertyMappingValue
                var propertyMappingValue = mappingDictionary[propertyName];
                
                if (propertyName == null)
                {
                    throw new ArgumentNullException("propertyMappingValue");
                }

                foreach (var destinationProperty in
                    propertyMappingValue.DestinationProperties.Reverse())
                {
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }

                    source = source.OrderBy(destinationProperty +
                        (orderDescending ? " descending" : " ascending"));
                }
            }

            return source;

        }
    }
}
