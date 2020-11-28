using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.Helpers
{
    public class EntityValidationHelper
    {
        

        public static ICollection<string> Validate(object instance)
        {
            ICollection<string> validationErrors = new List<string>();

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext =
                new ValidationContext(instance, null, null) ;
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
