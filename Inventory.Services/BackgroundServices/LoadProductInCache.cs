using Inventory.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Inventory.Services;

public class LoadProductInCache : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public LoadProductInCache(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var productService =
            scope.ServiceProvider.GetRequiredService<IProductService>();

        var res = await productService.GetAllAsync("");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var memoryCacheService =
            scope.ServiceProvider.GetRequiredService<IMemoryCacheService>();

        await memoryCacheService.RemoveAsync("Product");
    }


}
