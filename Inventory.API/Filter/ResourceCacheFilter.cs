using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Inventory.API.Filter;

public class ResourceCacheFilter : Attribute, IAsyncResourceFilter
{
    private readonly string _cacheKey;
    private readonly int _durationMinutes;

    public ResourceCacheFilter(string cacheKey, int durationMinutes)
    {
        _cacheKey = cacheKey;
        _durationMinutes = durationMinutes;


    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {

        if (!HttpMethods.IsGet(context.HttpContext.Request.Method))
        {
            await next();
            return;
        }

        var cacheService = context.HttpContext.RequestServices
                    .GetRequiredService<IMemoryCacheService>();

        var result = await cacheService.GetAsync<ObjectResult>(_cacheKey);

        if (result != null)
        {
            context.Result = result;
            return;
        }

        var actionContext = await next();

        if (actionContext.Result is ObjectResult actionResult &&
            actionResult.StatusCode == 200 &&
            actionResult.Value is not null)
        {
            cacheService.SetAsync(_cacheKey, actionContext.Result, TimeSpan.FromMinutes(_durationMinutes));
        }
    }

}