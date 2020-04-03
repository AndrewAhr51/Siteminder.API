using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Helper
{
    public static class DateTimeOffsetExtension
    {
        public static int GetCurrentAge(this DateTimeOffset dateTimeOffset, DateTimeOffset? dateOfDeath)
        {
            var currentDate = DateTime.UtcNow;

            if (dateOfDeath != null)
            {
                currentDate = dateOfDeath.Value.UtcDateTime;
            }

            int age = currentDate.Year - dateTimeOffset.Year;

            if (currentDate< dateTimeOffset.AddYears(age))
            {
                age--;
            }

            return age;
        }
    }
}
