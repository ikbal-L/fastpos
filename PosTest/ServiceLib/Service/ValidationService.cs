using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServiceLib.Service
{
    public class ValidationService
    {

        public static ICollection<string> Validate<T>(T instance)
        {
            ICollection<string> validationErrors = new List<string>();

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext =
                new ValidationContext(instance, null, null);
            if (!Validator.TryValidateObject(instance, validationContext, validationResults))
            {

                foreach (ValidationResult validationResult in validationResults)
                {
                    validationErrors.Add(validationResult.ErrorMessage);
                }
            }

            return validationErrors;

        }
    }
}