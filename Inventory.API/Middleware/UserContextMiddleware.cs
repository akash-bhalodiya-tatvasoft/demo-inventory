using Inventory.Common.Helpers;
using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Inventory.API.Middleware
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var email = context.User.GetUserEmail();
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var _userService = context.RequestServices.GetRequiredService<IUserService>();

                    var user = await _userService.GetUserByEmailAsync(email);
                    if (user != null)
                    {
                        context.Items["UserId"] = user.Id;
                        context.Items["UserEmail"] = user.Email;
                    }
                }
            }

            await _next(context);
        }
    }
}
