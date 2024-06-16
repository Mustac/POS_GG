using System;
using System.ComponentModel.DataAnnotations;

namespace POS_OS_GG.Helpers
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _conditionPropertyName;
        private readonly int _minimumLength;
        private readonly int _maximumLength;

        public RequiredIfAttribute(string conditionPropertyName, int minimumLength, int maximumLength)
        {
            _conditionPropertyName = conditionPropertyName;
            _minimumLength = minimumLength;
            _maximumLength = maximumLength;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_conditionPropertyName);
            if (property == null)
            {
                return new ValidationResult($"Unknown property: {_conditionPropertyName}");
            }

            var conditionValue = (bool)property.GetValue(validationContext.ObjectInstance);

            if (conditionValue)
            {
                if (value == null)
                {
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required.");
                }

                var stringValue = value as string;
                if (stringValue == null)
                {
                    return new ValidationResult($"{validationContext.DisplayName} must be a string.");
                }

                if (stringValue.Length < _minimumLength || stringValue.Length > _maximumLength)
                {
                    return new ValidationResult($"{validationContext.DisplayName} must be between {_minimumLength} and {_maximumLength} characters.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
