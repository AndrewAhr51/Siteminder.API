using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Siteminder.API.Models;

namespace Siteminder.API.ValidationAttributes
{
    public class PhoneMustBeDifferentFromFax: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var contact = (ContactForManipulationDto)validationContext.ObjectInstance;

            if (contact.Phone == contact.Fax)
            {
                return new ValidationResult(ErrorMessage,
                new[] {nameof(ContactForManipulationDto) });
            }

            return ValidationResult.Success;
        }
    }
}
