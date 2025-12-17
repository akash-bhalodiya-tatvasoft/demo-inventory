using Inventory.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Inventory.Services;

public class UserInactiveService : BackgroundService
{
    private readonly ILogger<UserInactiveService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public UserInactiveService(ILogger<UserInactiveService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var userService =
                    scope.ServiceProvider.GetRequiredService<IUserService>();

                var inactiveUsers = await userService.MarkUserInactive(3);

                await Task.Delay(1000 * 60 * 60, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");
        }
    }
}
