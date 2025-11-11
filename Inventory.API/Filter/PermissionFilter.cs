
using System.Net;
using System.Text.Json;
using Inventory.Common.Helpers;
using Inventory.Common.Responses;
using Inventory.Models.Entities;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static Inventory.Common.Enums.GlobalEnum;

namespace Inventory.API.Filter;

public class PermissionFilter : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly OperationType _opType;
    public PermissionFilter(OperationType opType = OperationType.View)
    {
        _opType = opType;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var email = context.HttpContext.User.GetUserEmail();
        if (string.IsNullOrWhiteSpace(email))
        {
            context.Result = new ObjectResult(ApiResponse<string>.Failure(
                                                                StatusCodes.Status401Unauthorized,
                                                                "Unauthorized"
                                                            ));
        }

        var _userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

        var user = await _userService.GetUserByEmailAsync(email);

        if (user == null)
        {
            context.Result = new ObjectResult(ApiResponse<string>.Failure(
                                                                StatusCodes.Status401Unauthorized,
                                                                "Unauthorized"
                                                            ));
        }

        var permission = JsonSerializer.Deserialize<UserPermissions>(user.Permissions);

        if (permission != null)
        {
            if ((_opType == OperationType.Create && !permission.Create) || (_opType == OperationType.Update && !permission.Update) || (_opType == OperationType.Delete && !permission.Delete))
            {
                context.Result = new ObjectResult(ApiResponse<string>.Failure(
                                                    StatusCodes.Status403Forbidden,
                                                    "Operation not permitted."
                                                ));
                return;
            }
        }
        else
        {
            context.Result = new ObjectResult(ApiResponse<string>.Failure(
                                                StatusCodes.Status403Forbidden,
                                                "Operation not permitted."
                                            ));
            return;
        }
    }
}