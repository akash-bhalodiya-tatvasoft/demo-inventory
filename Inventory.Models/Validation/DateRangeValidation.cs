using System.ComponentModel.DataAnnotations;
using Inventory.Models.Dashboard;

namespace Inventory.Models.Validation;

public class DateRangeValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DashboardRequest request)
        {
            if (request.FromDate > request.ToDate)
            {
                return new ValidationResult(
                    "FromDate must be earlier than or equal to ToDate.",
                    new[] { nameof(request.FromDate), nameof(request.ToDate) }
                );
            }
        }

        return ValidationResult.Success;
    }
}
