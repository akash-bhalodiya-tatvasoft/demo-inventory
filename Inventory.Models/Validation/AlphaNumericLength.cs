using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Inventory.Models.Validation;

public class AlphaNumericMinLengthAttribute : ValidationAttribute
{
    private readonly int _minLength;

    public AlphaNumericMinLengthAttribute(int minLength)
    {
        _minLength = minLength;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string str || string.IsNullOrWhiteSpace(str))
        {
            return new ValidationResult($"{validationContext.DisplayName} is required.");
        }

        if (str.Length < _minLength)
        {
            return new ValidationResult(
                $"{validationContext.DisplayName} must be at least {_minLength} characters long."
            );
        }

        if (!Regex.IsMatch(str, @"^[a-zA-Z0-9]+$"))
        {
            return new ValidationResult(
                $"{validationContext.DisplayName} can contain only letters and digits."
            );
        }

        return ValidationResult.Success;
    }
}
