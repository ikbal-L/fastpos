using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Attributes
{

    [AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Parameter | System.AttributeTargets.Property)]
    public class MinAttribute: ValidationAttribute
    {
        public double DoubleMinValue { get; }
        public decimal DecimalMinValue { get; }

        public MinAttribute(string decimalValue)
        {
            DecimalMinValue = decimal.Parse(decimalValue);
        }

        public MinAttribute(double doubleMinValue)
        {
            DoubleMinValue = doubleMinValue;
        }

        public string GetErrorMessage(object obj, string displayName = "") => $" Min Value of {displayName} must be {obj}.";



        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            if(value is double doubleValue && doubleValue < DoubleMinValue)
            {
                return  new ValidationResult(GetErrorMessage(DoubleMinValue,validationContext.DisplayName));
            }

            if (value is decimal decimalValue && decimalValue < DecimalMinValue)
            {
                return new ValidationResult(GetErrorMessage(DecimalMinValue,validationContext.DisplayName));
            }

            if (value is string stringValue)
            {
                if (decimal.TryParse(stringValue,out decimalValue))
                {
                    if (decimalValue<DecimalMinValue)
                    {
                        return new ValidationResult(GetErrorMessage(DecimalMinValue, validationContext.DisplayName));
                    }
                }

            }
            
            return ValidationResult.Success;
        }
    }

    public class DecimalAttribute : ValidationAttribute
    {

        public string GetErrorMessage(object obj,string displayName ="") => $"Value of {displayName} must be of Type {nameof(Decimal)}.";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            if (value is string s && decimal.TryParse(s,out  var d))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(GetErrorMessage(value, validationContext.DisplayName));
        }
    }


    public class Collection
    {
        [AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Parameter | System.AttributeTargets.Property)]
        public class NonEmptyAttribute:ValidationAttribute
        {
        
            
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is IEnumerable enumerable)
                {
                    if (!enumerable.GetEnumerator().MoveNext())
                    {
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    }
                    return ValidationResult.Success;
                }

                var message = $"Property {validationContext.DisplayName} on Type {validationContext.ObjectType.FullName} must be of type that implements ${nameof(IEnumerable)}";
#if DEBUG
                throw new InvalidOperationException(message); 
#endif
                return new ValidationResult(message);
            }

            public override string FormatErrorMessage(string name)
            {
                return $"Collection {name} must not be empty";
            }
        }


        
    }
}