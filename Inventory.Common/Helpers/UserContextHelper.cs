using System.Security.Claims;

namespace Inventory.Common.Helpers;

public static class UserContextHelper
{

    public static string? GetUserEmail(this ClaimsPrincipal user)
    {
        if (user == null) return null;

        var email = user.FindFirst(ClaimTypes.Email)?.Value;

        return email;
    }
}
