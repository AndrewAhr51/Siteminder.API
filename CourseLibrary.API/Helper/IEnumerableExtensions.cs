using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Siteminder.API.Helper
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(
            this IEnumerable<TSource> source,
            string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expandoObjectList = new List<ExpandoObject>();
            // create a list with the PropertInfo objects on TSource. Reflection is
            // expensive, so rather the doingit for each object in the list , we do
            // it once and reuse the results.  After all, part of the reflection is on the
            // type of the object (TSource), not on the instance
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfo = typeof(TSource)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfo);

            }
            else
            {
                // the field are seperated by ",", so we split it
                var fieldsAfterSplit = fields.Split(',');

                foreach(var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();

                    var propertyInfo = typeof(TSource)
                   .GetProperty(propertyName,BindingFlags.IgnoreCase 
                   | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception ($"Property{propertyName} wasn't found on {typeof(TSource)}");
                    }

                    // addpropertyInfo to list
                    propertyInfoList.Add(propertyInfo);
                }
            }

            // run through the source objects
            foreach(TSource sourceObject in source)
            {
                // create an ExpandoObject that will hold the
                // selected properties and values
                var dataShapedObject = new ExpandoObject();

                // get the value of each property we have to return.  For that,
                // we run through the list
                foreach(var propertyInfo in propertyInfoList)
                {
                    // getValue returns the value of theproperty on the source object
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    //add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject)
                       .Add(propertyInfo.Name, propertyValue);
                }

                // add the ExpandoObject to the list
                expandoObjectList.Add(dataShapedObject);
            }

            // return the list
            return expandoObjectList;
        }
    }
}
