using Microsoft.AspNetCore.Authorization;
using Inventory.Common.Enums;

namespace Inventory.Api.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        string claimType = string.Empty;

        switch (requirement.Operation)
        {
            case GlobalEnum.OperationType.Create:
                claimType = "permission.create";
                break;
            case GlobalEnum.OperationType.Update:
                claimType = "permission.update";
                break;
            case GlobalEnum.OperationType.Delete:
                claimType = "permission.delete";
                break;
        }

        var permissionClaim = context.User.FindFirst(claimType)?.Value;

        if (permissionClaim == "True")
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
