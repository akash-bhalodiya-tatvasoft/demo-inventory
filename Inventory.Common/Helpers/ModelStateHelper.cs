using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inventory.Common.Helpers;

public static class ModelStateHelper
{

    public static object ToErrorResponse(this ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return errors;
    }
}
