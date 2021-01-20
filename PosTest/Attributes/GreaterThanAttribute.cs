using System.ComponentModel.DataAnnotations;

namespace Attributes
{

    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Parameter | System.AttributeTargets.Property)]
    public class GreaterThanAttribute: ValidationAttribute
    {
        public double DoubleValue { get; }
        public decimal DecimalValue { get; }

        public GreaterThanAttribute(string decimalValue)
        {
            DecimalValue = decimal.Parse(decimalValue);
        }

        public GreaterThanAttribute(double doubleValue)
        {
            DoubleValue = doubleValue;
        }

        public string GetErrorMessage(object obj) => $"Value must be Greater  than {obj}.";



        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            if(value is double doubleValue && doubleValue <= DoubleValue)
            {
                return  new ValidationResult(GetErrorMessage(DoubleValue));
            }

            if (value is decimal decimalValue && decimalValue <= DecimalValue)
            {
                return new ValidationResult(GetErrorMessage(DecimalValue));
            }
            
            return ValidationResult.Success;
        }
    }
}