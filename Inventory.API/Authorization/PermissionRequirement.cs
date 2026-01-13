using Microsoft.AspNetCore.Authorization;
using Inventory.Common.Enums;

namespace Inventory.Api.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public GlobalEnum.OperationType Operation { get; }

    public PermissionRequirement(GlobalEnum.OperationType operation)
    {
        Operation = operation;
    }
}
