using Inventory.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Inventory.API.Filter;

public class ResourceCacheFilter : Attribute, IAsyncResourceFilter
{
    private readonly string _cacheKey;
    private readonly int _durationMinutes;
    private readonly bool _includeQueryParams;

    public ResourceCacheFilter(
        string cacheKey,
        int durationMinutes,
        bool includeQueryParams = false)
    {
        _cacheKey = cacheKey;
        _durationMinutes = durationMinutes;
        _includeQueryParams = includeQueryParams;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {

        if (!HttpMethods.IsGet(context.HttpContext.Request.Method))
        {
            await next();
            return;
        }

        var request = context.HttpContext.Request;

        if (!_includeQueryParams && request.Query.Any())
        {
            await next();
            return;
        }

        var cacheService = context.HttpContext.RequestServices
            .GetRequiredService<IMemoryCacheService>();

        var finalCacheKey = _includeQueryParams && request.Query.Any()
            ? $"{_cacheKey}:{request.Path}:{request.QueryString}"
            : $"{_cacheKey}:{request.Path}";

        var result = await cacheService.GetAsync<ObjectResult>(finalCacheKey);

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
            await cacheService.SetAsync(finalCacheKey, actionContext.Result, TimeSpan.FromMinutes(_durationMinutes));
        }
    }

}